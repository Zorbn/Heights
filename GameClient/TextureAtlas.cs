using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient;

public class TextureAtlas
{
    public readonly int Width;
    public readonly int Height;

    private readonly Texture2D texture;
    public readonly int TileSize;
    public readonly int HalfTileSize;

    public TextureAtlas(GraphicsDevice graphicsDevice, string path, int tileSize)
    {
        texture = Texture2D.FromFile(graphicsDevice, path);
        Width = texture.Width / tileSize;
        Height = texture.Height / tileSize;
        TileSize = tileSize;
        HalfTileSize = TileSize / 2;
    }

    public void Draw(SpriteBatch batch, Camera camera, Vector2 position, int texX, int texY, int texW, int texH,
        Color color, Vector2? scale = null, float rotation = 0f, bool flipped = false)
    {
        Vector2 size = (scale ?? Vector2.One) * camera.Scale;
        var srcRect = new Rectangle(texX * TileSize, texY * TileSize, texW * TileSize, texH * TileSize);
        var halfSize = new Vector2(srcRect.Width * 0.5f, srcRect.Height * 0.5f);
        Vector2 drawPos = (position + halfSize - camera.Position) * camera.Scale;
        // Round the position to prevent artifacts when drawing at subpixel offsets.
        drawPos.Round();
        SpriteEffects spriteEffects = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        batch.Draw(texture, drawPos, srcRect, color, rotation, halfSize, size, spriteEffects, 0f);
    }

    public int GetAtlasSize(float size)
    {
        return (int)(size / TileSize);
    }
}