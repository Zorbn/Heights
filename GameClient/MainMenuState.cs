using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameClient;

public class MainMenuState : IGameState
{
    private GameClient gameClient;
    private Camera camera;
    private string ipText = "";
    
    public void Initialize(GameClient newGameClient, int screenWidth, int screenHeight, params string[] args)
    {
        gameClient = newGameClient;
        camera = new Camera(screenWidth, screenHeight, false);
    }

    public void Update(Input input, float deltaTime)
    {
        if (input.WasKeyPressed(Keys.Enter))
        {
            string ip = ipText.ToLower();
            gameClient.SwitchGameState(GameState.Connecting, ip);
            return;
        }
        
        if (ipText.Length > 0 && input.WasKeyPressed(Keys.Back))
        {
            ipText = input.IsKeyDown(Keys.LeftControl) ? "" : ipText.Remove(ipText.Length - 1);
        }
        
        foreach (Keys heldKey in input.CurrentState.GetPressedKeys())
        {
            if (!input.WasKeyPressed(heldKey)) continue;

            var keyIndex = (int)heldKey;
            bool keyIsLetter = keyIndex is >= (int)Keys.A and <= (int)Keys.Z;
            bool keyIsNumber = keyIndex is >= (int)Keys.D0 and <= (int)Keys.D9;
            bool keyIsPeriod = heldKey == Keys.OemPeriod;
            bool keyIsValid = keyIsLetter || keyIsNumber || keyIsPeriod;
            if (!keyIsValid) return;

            var keyString = heldKey.ToString();
            char keyChar = keyIsPeriod ? '.' : keyString[^1];
            ipText += keyChar;
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
        string drawableIpText = ipText;

        int ipTextY = atlas.TileSize * 3 + atlas.HalfTileSize * 3;
        int playTextY = ipTextY + atlas.TileSize * 2 + atlas.HalfTileSize * 2;
        
        if (ipText.Length == 0)
        {
            drawableIpText = "TYPE AN IP...";
        }
        else
        {
            TextRenderer.Draw(":HIT ENTER TO PLAY", atlas.HalfTileSize, playTextY, atlas, batch, camera);
        }
        
        TextRenderer.Draw(drawableIpText, atlas.HalfTileSize, ipTextY, atlas, batch, camera);

        batch.End();
    }

    public void Dispose()
    {
    }
}