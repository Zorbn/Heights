using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;

namespace GameClient;

public class MainMenuState : IGameState
{
    private GameClient gameClient;
    private Camera camera;
    private MainMenuSubState subState = MainMenuSubState.Name;
    private string nameText = "";
    private string ipText = "";
    
    public void Initialize(GameClient newGameClient, int screenWidth, int screenHeight, params string[] args)
    {
        gameClient = newGameClient;
        camera = new Camera(screenWidth, screenHeight);
    }

    public void Update(Input input, float deltaTime)
    {
        switch (subState)
        {
            case MainMenuSubState.Name:
                UpdateName(input, deltaTime);
                break;
            case MainMenuSubState.Ip:
                UpdateIp(input, deltaTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Draw(Background background, TextureAtlas atlas, SpriteBatch batch, int windowWidth, int windowHeight, float deltaTime)
    {
        camera.ScaleToScreen(windowWidth, windowHeight);
        
        batch.Begin(samplerState: SamplerState.PointClamp);

        background.Draw(batch, camera);

        // Draw the title.
        var titlePos = new Vector2(atlas.HalfTileSize + atlas.TileSize * 3.5f, atlas.HalfTileSize + atlas.TileSize * 0.5f);
        var titleScale = new Vector2(2f);
        Vector2 titleShadowPos = titlePos + new Vector2(atlas.TileSize, 0f) * 2f;
        Vector2 titleShadowScale = new Vector2(7, 1.5f) * titleScale;
        atlas.Draw(batch, camera, titleShadowPos, 8, 11, 1, 1, Color.White, titleShadowScale);
        atlas.Draw(batch, camera, titlePos, 9, 11, 7, 1, Color.White, titleScale);

        // Draw text boxes.
        string drawableText = subState == MainMenuSubState.Ip ? ipText : nameText;

        int inputTextY = atlas.TileSize * 3 + atlas.HalfTileSize * 3;
        int continueTextY = inputTextY + atlas.TileSize * 2 + atlas.HalfTileSize * 2;

        drawableText = PrepareDrawableText(drawableText,
            subState == MainMenuSubState.Ip ? "TYPE AN IP..." : "TYPE A NAME...", atlas, batch, continueTextY);

        TextRenderer.Draw(drawableText, atlas.HalfTileSize, inputTextY, atlas, batch, camera);

        batch.End();
    }

    private string PrepareDrawableText(string text, string placeholder, TextureAtlas atlas, SpriteBatch batch, int continueTextY)
    {
        if (text.Length == 0)
        {
            return placeholder;
        }

        TextRenderer.Draw(":PRESS ENTER", atlas.HalfTileSize, continueTextY, atlas, batch, camera);
        return text;
    }

    private void UpdateName(Input input, float deltaTime)
    {
        if (input.WasKeyPressed(Keys.Enter))
        {
            subState = MainMenuSubState.Ip;
            return;
        }

        nameText = TextBoxInput(nameText, input);
    }
    
    private void UpdateIp(Input input, float deltaTime)
    {
        if (input.WasKeyPressed(Keys.Enter))
        {
            string ip = ipText.ToLower();
            string name = nameText.Trim();

            if (name.Length == 0)
            {
                name = Player.DefaultName;
            }
            
            gameClient.SwitchGameState(GameState.Connecting, ip, name);
            return;
        }

        ipText = TextBoxInput(ipText, input);
    }

    private static string TextBoxInput(string text, Input input)
    {
        if (text.Length > 0 && input.WasKeyPressed(Keys.Back))
        {
            text = input.IsKeyDown(Keys.LeftControl) ? "" : text.Remove(text.Length - 1);
        }
        
        foreach (Keys heldKey in input.CurrentState.GetPressedKeys())
        {
            if (!input.WasKeyPressed(heldKey)) continue;

            var keyIndex = (int)heldKey;
            bool keyIsLetter = keyIndex is >= (int)Keys.A and <= (int)Keys.Z;
            bool keyIsNumber = keyIndex is >= (int)Keys.D0 and <= (int)Keys.D9;
            bool keyIsPeriod = heldKey == Keys.OemPeriod;
            bool keyIsValid = keyIsLetter || keyIsNumber || keyIsPeriod;
            if (!keyIsValid) return text;

            var keyString = heldKey.ToString();
            char keyChar = keyIsPeriod ? '.' : keyString[^1];
            text += keyChar;
        }

        return text;
    }

    public void Dispose()
    {
    }
}