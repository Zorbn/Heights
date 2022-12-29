using System;
using System.Collections.Generic;
using System.Linq;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;

namespace FastJump;

/* TODO:
 * Menu to choose IP and start the game.
 * Goal flag that the players try to reach.
 * Score/time that is awarded to players when they reach the goal.
 * Make the map larger/improve the map's textures.
 * Change the player's sprite when moving.
 */

public class FastJumpGame : Game
{
    public const int VirtualScreenWidth = 320;
    public const int VirtualScreenHeight = 180;
    public const int WindowDefaultSizeMultiplier = 2;
    
    private const float SpriteInterpSpeed = 20f;
    
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private TextureAtlas textureAtlas;
    
    private Dictionary<int, PlayerData> players = new();
    private Map map;
    private Camera camera;
    private int localId = -1;
    
    public FastJumpGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        InactiveSleepTime = TimeSpan.Zero; // Don't throttle FPS when the window is inactive.
        graphics.PreferredBackBufferWidth = VirtualScreenWidth * WindowDefaultSizeMultiplier;
        graphics.PreferredBackBufferHeight = VirtualScreenHeight * WindowDefaultSizeMultiplier;
        graphics.ApplyChanges();
    }
    
    protected override void Initialize()
    {
        textureAtlas = new TextureAtlas(GraphicsDevice, "Content/atlas.png", 8);
        map = new Map("Content/map.json");
        camera = new Camera(VirtualScreenWidth, VirtualScreenHeight);
     
        Dictionary<Message.MessageType, MessageStream.MessageHandler> messageHandlers = new()
        {
            { Message.MessageType.ExampleNotification, ExampleNotification.HandleNotification },
            { Message.MessageType.SpawnPlayer, HandleSpawnPlayer },
            { Message.MessageType.DestroyPlayer, HandleDestroyPlayer },
            { Message.MessageType.MovePlayer, HandleMovePlayer }
        };
        
        Client.StartClient("127.0.0.1", messageHandlers, OnDisconnect, OnConnect, OnConnectFailed, OnInitialized);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyState.IsKeyDown(Keys.Escape))
            Exit();

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keyState.IsKeyDown(Keys.S))
        {
            SendExampleNotification();
        }

        LocalUpdate(keyState, deltaTime);

        base.Update(gameTime);
    }

    private void LocalUpdate(KeyboardState keyState, float deltaTime)
    {
        if (localId == -1) return;
        if (!players.TryGetValue(localId, out PlayerData playerData)) return;

        Player player = playerData.Player;

        var dir = 0f;
        if (keyState.IsKeyDown(Keys.Left)) dir -= 1f;
        if (keyState.IsKeyDown(Keys.Right)) dir += 1f;
        bool tryJump = keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Up);
        bool noClip = keyState.IsKeyDown(Keys.N);
        player.Move(dir, tryJump, noClip, map.MapData, deltaTime);
        
        Client.SendMessage(Message.MessageType.MovePlayer, new MovePlayerData
        {
            Id = localId,
            X = player.Position.X,
            Y = player.Position.Y,
            Direction = (byte)player.Direction
        });
    }

    protected override void Draw(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        camera.ScaleToScreen(Window.ClientBounds.Width, Window.ClientBounds.Height);
        
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        map.Draw(textureAtlas, spriteBatch, camera);

        KeyValuePair<int, PlayerData>[] drawablePlayers = players.ToArray();
        foreach (KeyValuePair<int, PlayerData> pair in drawablePlayers)
        {
            if (pair.Key == localId)
            {
                pair.Value.Sprite.Teleport(pair.Value.Player.Position);
            }
            else
            {
                pair.Value.Sprite.StepTowards(pair.Value.Player.Position, deltaTime * SpriteInterpSpeed);
            }

            bool flipped = pair.Value.Player.Direction == Direction.Left;
            textureAtlas.Draw(spriteBatch, camera, pair.Value.Sprite.Position, 0, 0, 2, 2, Color.White, 1f, 0f, flipped);
        }
        
        spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandleSpawnPlayer(int fromId, Data data)
    {
        if (data is not SpawnPlayerData spawnData) return;

        var playerPos = new Vector2(spawnData.X, spawnData.Y);
        
        players.Add(spawnData.Id, new PlayerData
        {
            Player = new Player(playerPos),
            Sprite = new Sprite(playerPos)
        });
    }
    
    private void HandleDestroyPlayer(int fromId, Data data)
    {
        if (data is not DestroyPlayerData destroyData) return;
        
        players.Remove(destroyData.Id);
    }
    
    private void HandleMovePlayer(int fromId, Data data)
    {
        if (data is not MovePlayerData moveData) return;

        Player player = players[moveData.Id].Player;
        player.Position.X = moveData.X;
        player.Position.Y = moveData.Y;
        player.Direction = (Direction)moveData.Direction;
    }
    
    public void OnDisconnect(int id)
    {
        Console.WriteLine("Disconnected!");
        localId = -1;
    }

    public void OnConnect()
    {
        Console.WriteLine("Connected!");
    }

    public void OnConnectFailed()
    {
        Console.WriteLine("Connection failed!");
        Exit();
    }

    public void OnInitialized(int localId)
    {
        this.localId = localId;
    }

    private void SendExampleNotification()
    {
        ExampleNotificationData exampleNotificationData = new()
        {
            Text = "aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaa1000"
        };
            
        Client.SendMessage(Message.MessageType.ExampleNotification, exampleNotificationData);
    }
}