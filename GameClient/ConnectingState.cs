using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameClient;

public class ConnectingState : IGameState
{
    private GameClient gameClient;
    private int localId = -1;

    private Camera camera;

    public void Initialize(GameClient newGameClient, int screenWidth, int screenHeight, params string[] args)
    {
        gameClient = newGameClient;

        Dictionary<Message.MessageType, MessageStream.MessageHandler> messageHandlers = new();
        
        try
        {
            Client.StartClient(args[0], messageHandlers, OnDisconnect, OnConnect, OnConnectFailed, OnInitialized);
        }
        catch (SocketException)
        {
            gameClient.SwitchGameState(GameState.MainMenu);
        }
        
        camera = new Camera(screenWidth, screenHeight, false);
    }

    public void Update(Input input, float deltaTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            input.IsKeyDown(Keys.Escape))
            gameClient.SwitchGameState(GameState.MainMenu);
    }

    public void Draw(Background background, TextureAtlas atlas, SpriteBatch batch, int windowWidth, int windowHeight, float deltaTime)
    {
        camera.ScaleToScreen(windowWidth, windowHeight);
        
        batch.Begin(samplerState: SamplerState.PointClamp);
        
        background.Draw(batch, camera);
        
        TextRenderer.Draw("Connecting...", atlas.HalfTileSize, camera.ViewHeight - atlas.TileSize - atlas.HalfTileSize, atlas, batch, camera);
        
        batch.End();
    }

    public void Dispose()
    {
    }
    
    private void OnDisconnect(int id)
    {
        Console.WriteLine("Disconnected!");
        localId = -1;
    }

    private void OnConnect()
    {
        Console.WriteLine("Connected!");
    }

    private void OnConnectFailed()
    {
        Console.WriteLine("Connection failed!");
        gameClient.SwitchGameState(GameState.MainMenu);
    }

    private void OnInitialized(int newLocalId)
    {
        localId = newLocalId;
        gameClient.SwitchGameState(GameState.InGame, localId.ToString());
    }
}