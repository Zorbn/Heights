using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient;

public class Background
{
    // Slow the background's movement to make it look far away.
    private const float MovementFactor = 0.2f;

    public readonly Color Color;
    private readonly Texture2D texture;
    private readonly int tiles;
    private readonly Vector2 maxFactor;

    public Background(GraphicsDevice graphicsDevice, string path, int tiles = 3)
    {
        texture = Texture2D.FromFile(graphicsDevice, path);
        this.tiles = tiles;
        maxFactor = new Vector2(tiles * 0.5f * texture.Width, 0.5f * texture.Width);
        
        var outColor = new Color[1];
        texture.GetData(0, new Rectangle(0, texture.Height - 1, 1, 1), outColor, 0, 1);
        Color = outColor[0];
    }

    public void Draw(SpriteBatch batch, Camera camera)
    {
        var scale = new Vector2(camera.Scale);
        var halfSize = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
        Vector2 drawPos = (-camera.Position * MovementFactor * 0.1f + new Vector2(camera.ViewWidth, camera.ViewHeight) * 0.5f) * camera.Scale;
        
        Vector2 maxPos = maxFactor * camera.Scale;
        drawPos.X = Math.Clamp(drawPos.X, -maxPos.X, maxPos.X);
        drawPos.Y = Math.Clamp(drawPos.Y, -maxPos.Y, maxPos.Y);

        int halfTiles = tiles / 2;
        for (int i = -halfTiles; i <= halfTiles; i++)
        {
            var offset = new Vector2(i * texture.Width * camera.Scale, 0f);
            batch.Draw(texture, drawPos + offset, null, Color.White, 0f, halfSize, scale, SpriteEffects.None, 0f);
        }
    }
}