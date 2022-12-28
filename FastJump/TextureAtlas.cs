using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FastJump;

public class TextureAtlas
{
    private readonly Texture2D texture;
    public readonly int TileSize;

    public TextureAtlas(GraphicsDevice graphicsDevice, string path, int tileSize)
    {
        texture = Texture2D.FromFile(graphicsDevice, path);
        TileSize = tileSize;
    }

    public void Draw(SpriteBatch batch, Vector2 position, int texX, int texY, int texW, int texH,
        Color color, float scale = 1f, float rotation = 0f)
    {
        var srcRect = new Rectangle(texX * TileSize, texY * TileSize, texW * TileSize, texH * TileSize);
        var halfSize = new Vector2(srcRect.Width * 0.5f, srcRect.Height * 0.5f);
        Vector2 drawPos = position + halfSize;
        
        batch.Draw(texture, drawPos, srcRect, color, rotation, halfSize, scale, SpriteEffects.None, 0f);
    }
}