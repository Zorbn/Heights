using System;
using System.Collections.Generic;
using System.Linq;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;

namespace GameClient;

/* TODO:
 * Menu to choose IP and start the game.
 * Score/time that is awarded to players when they reach the goal.
 * Make the map larger.
 */

public class GameClient : Game
{
    private const int VirtualScreenWidth = 320;
    private const int VirtualScreenHeight = 180;
    private const int WindowDefaultSizeMultiplier = 2;

    private const float SpriteInterpSpeed = 20f;
    private const float CameraInterpSpeed = 20f;
    private Camera camera;

    private readonly GraphicsDeviceManager graphics;
    private int localId = -1;
    private Map map;

    private readonly Dictionary<int, PlayerData> players = new();
    private SpriteBatch spriteBatch;
    private TextureAtlas textureAtlas;
    private Camera uiCamera;

    public GameClient()
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
        textureAtlas = new TextureAtlas(GraphicsDevice, "Content/atlas.png", 16);
        Sprite.LoadAnimation(Animation.PlayerIdle, "Content/Animations/playerIdle.json");
        Sprite.LoadAnimation(Animation.PlayerRunning, "Content/Animations/playerRunning.json");
        Sprite.LoadAnimation(Animation.PlayerJumping, "Content/Animations/playerJumping.json");
        map = new Map("Content/map.json");
        camera = new Camera(VirtualScreenWidth, VirtualScreenHeight);
        uiCamera = new Camera(VirtualScreenWidth, VirtualScreenHeight, false);

        Dictionary<Message.MessageType, MessageStream.MessageHandler> messageHandlers = new()
        {
            { Message.MessageType.ExampleNotification, ExampleNotification.HandleNotification },
            { Message.MessageType.SpawnPlayer, HandleSpawnPlayer },
            { Message.MessageType.DestroyPlayer, HandleDestroyPlayer },
            { Message.MessageType.MovePlayer, HandleMovePlayer },
            { Message.MessageType.Heartbeat, HandleHeartbeat },
            { Message.MessageType.UpdateScore, HandleUpdateScore }
        };

        Client.StartClient("127.0.0.1", messageHandlers, OnDisconnect, OnConnect, OnConnectFailed, OnInitialized);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyState.IsKeyDown(Keys.Escape))
            Exit();

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keyState.IsKeyDown(Keys.S)) SendExampleNotification();

        LocalUpdate(keyState, deltaTime);

        base.Update(gameTime);
    }

    private bool TryGetLocalPlayer(out PlayerData playerData)
    {
        playerData = null;
        
        if (localId == -1) return false;
        return players.TryGetValue(localId, out playerData);
    }

    private void LocalUpdate(KeyboardState keyState, float deltaTime)
    {
        if (!TryGetLocalPlayer(out PlayerData playerData)) return;

        Player player = playerData.Player;

        var dir = 0f;
        if (keyState.IsKeyDown(Keys.Left)) dir -= 1f;
        if (keyState.IsKeyDown(Keys.Right)) dir += 1f;
        bool tryJump = keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Up);
        bool noClip = keyState.IsKeyDown(Keys.N);
        player.Move(dir, tryJump, noClip, map.MapData, deltaTime);
        camera.StepTowards(player.Position + new Vector2(map.MapData.TileSize * 0.5f), CameraInterpSpeed * deltaTime);

        Client.SendMessage(Message.MessageType.MovePlayer, new MovePlayerData
        {
            Id = localId,
            X = player.Position.X,
            Y = player.Position.Y,
            Direction = (byte)player.Direction,
            Animation = (byte)player.Animation
        });
    }

    protected override void Draw(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        camera.ScaleToScreen(Window.ClientBounds.Width, Window.ClientBounds.Height);
        uiCamera.ScaleToScreen(Window.ClientBounds.Width, Window.ClientBounds.Height);

        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        map.Draw(textureAtlas, spriteBatch, camera);

        KeyValuePair<int, PlayerData>[] drawablePlayers = players.ToArray();
        foreach (KeyValuePair<int, PlayerData> pair in drawablePlayers)
        {
            if (pair.Key == localId)
                pair.Value.Sprite.Teleport(pair.Value.Player.Position);
            else
                pair.Value.Sprite.StepTowards(pair.Value.Player.Position, deltaTime * SpriteInterpSpeed);

            bool flipped = pair.Value.Player.Direction == Direction.Left;
            Frame frame = pair.Value.Sprite.UpdateAnimation(pair.Value.Player.Animation, deltaTime);
            textureAtlas.Draw(spriteBatch, camera, pair.Value.Sprite.Position, frame.X, frame.Y, 2, 2, Color.White, 1f,
                0f, flipped);
        }

        if (TryGetLocalPlayer(out PlayerData playerData))
        {
            Player player = playerData.Player;
            TextRenderer.Draw($"SCORE {player.Score}", 8, 8, textureAtlas, spriteBatch, uiCamera);
        }

        spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        Client.SendMessage(Message.MessageType.Disconnect, new DisconnectData());
        Client.StopClient();

        base.Dispose(disposing);
    }

    private void HandleSpawnPlayer(int fromId, IData data)
    {
        if (data is not SpawnPlayerData spawnData) return;

        var playerPos = new Vector2(spawnData.X, spawnData.Y);

        players.Add(spawnData.Id, new PlayerData
        {
            Player = new Player(playerPos),
            Sprite = new Sprite(playerPos)
        });
    }

    private void HandleDestroyPlayer(int fromId, IData data)
    {
        if (data is not DestroyPlayerData destroyData) return;

        players.Remove(destroyData.Id);
    }

    private void HandleMovePlayer(int fromId, IData data)
    {
        if (data is not MovePlayerData moveData) return;

        Player player = players[moveData.Id].Player;
        player.Position.X = moveData.X;
        player.Position.Y = moveData.Y;
        player.Direction = (Direction)moveData.Direction;
        player.Animation = (Animation)moveData.Animation;
    }

    private void HandleHeartbeat(int fromId, IData data)
    {
        Client.SendMessage(Message.MessageType.Heartbeat, new HeartbeatData());
    }

    private void HandleUpdateScore(int fromId, IData data)
    {
        if (data is not UpdateScoreData scoreData) return;

        Player player = players[scoreData.Id].Player;
        player.Score = scoreData.Score;
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

    public void OnInitialized(int newLocalId)
    {
        localId = newLocalId;
    }

    private void SendExampleNotification()
    {
        ExampleNotificationData exampleNotificationData = new()
        {
            Text =
                "aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaaaa10aaaaaaa1000"
        };

        Client.SendMessage(Message.MessageType.ExampleNotification, exampleNotificationData);
    }
}