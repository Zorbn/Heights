using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FastJump;

public class TextureAtlas
{
    // Prevent textures occasionally bleeding together on the edges.
    public const float Padding = 0.01f;
    public readonly int TileSize;
    public readonly int Width;
    public readonly int Height;
    
    private readonly Texture2D texture;

    public TextureAtlas(GraphicsDevice graphicsDevice, string path, int tileSize)
    {
        texture = Texture2D.FromFile(graphicsDevice, path);
        Width = texture.Width / tileSize;
        Height = texture.Height / tileSize;
        TileSize = tileSize;
    }

    public void Draw(SpriteBatch batch, Camera camera, Vector2 position, int texX, int texY, int texW, int texH,
        Color color, float scale = 1f, float rotation = 0f, bool flipped = false)
    {
        if (camera.Cull)
        {
            bool notVisibleX = position.X > camera.Position.X + camera.ViewWidth ||
                               position.X + texW * TileSize < camera.Position.X;

            bool notVisibleY = position.Y > camera.Position.Y + camera.ViewHeight ||
                               position.Y + texH * TileSize < camera.Position.Y;

            if (notVisibleX || notVisibleY) return;
        }

        var size = new Vector2(scale * camera.Scale);
        var srcRect = new Rectangle(texX * TileSize, texY * TileSize, texW * TileSize, texH * TileSize);
        var halfSize = new Vector2(srcRect.Width * 0.5f, srcRect.Height * 0.5f);
        Vector2 drawPos = (position + halfSize - camera.Position) * camera.Scale;
        SpriteEffects spriteEffects = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        
        batch.Draw(texture, drawPos, srcRect, color, rotation, halfSize, size, spriteEffects, 0f);
    }

    public int GetAtlasSize(float size)
    {
        return (int)(size / TileSize);
    }
}