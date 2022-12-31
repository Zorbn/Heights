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

    public void Draw(TextureAtlas atlas, SpriteBatch batch, int windowWidth, int windowHeight, float deltaTime)
    {
        camera.ScaleToScreen(windowWidth, windowHeight);
        
        batch.Begin(samplerState: SamplerState.PointClamp);

        int padding = atlas.TileSize / 2;
        var titlePos = new Vector2(padding + atlas.TileSize * 3.5f, padding + atlas.TileSize * 0.5f);
        atlas.Draw(batch, camera, titlePos, 9, 11, 7, 1, Color.White, 2f);

        string drawableIpText = ipText;

        int ipTextY = atlas.TileSize * 3 + padding * 3;
        int playTextY = ipTextY + atlas.TileSize * 2 + padding * 2;
        
        if (ipText.Length == 0)
        {
            drawableIpText = "TYPE AN IP...";
        }
        else
        {
            TextRenderer.Draw(":HIT ENTER TO PLAY", padding, playTextY, atlas, batch, camera);
        }
        
        TextRenderer.Draw(drawableIpText, padding, ipTextY, atlas, batch, camera);

        batch.End();
    }

    public void Dispose()
    {
    }
}