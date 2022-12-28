using System;
using System.Collections.Generic;
using System.Linq;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;

namespace FastJump;

public class FastJumpGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private TextureAtlas textureAtlas;
    
    private Dictionary<int, PlayerData> players = new();
    private const float PlayerSpeed = 100f;
    private const float SpriteInterpSpeed = 10f;
    private int localId = -1;
    
    public FastJumpGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        textureAtlas = new TextureAtlas(GraphicsDevice, "Content/atlas.png", 8);
     
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
        
        Vector2 move = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.Left))
        {
            move.X -= 1f;
        }

        if (keyState.IsKeyDown(Keys.Right))
        {
            move.X += 1f;
        }

        if (keyState.IsKeyDown(Keys.Up))
        {
            move.Y -= 1f;
        }

        if (keyState.IsKeyDown(Keys.Down))
        {
            move.Y += 1f;
        }

        if (move.Length() != 0f)
        {
            move.Normalize();
            player.Position += move * PlayerSpeed * deltaTime;
            Client.SendMessage(Message.MessageType.MovePlayer, new MovePlayerData
            {
                Id = localId,
                X = player.Position.X,
                Y = player.Position.Y
            });
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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

            textureAtlas.Draw(spriteBatch, pair.Value.Sprite.Position, 0, 0, 2, 2, Color.White);
        }

        textureAtlas.Draw(spriteBatch, new Vector2(16, 0), 2, 0, 2, 2, Color.White);
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