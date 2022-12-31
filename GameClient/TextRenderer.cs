using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient;

public class TextRenderer
{
    public const int TextureXStart = 2;
    public const int TextureYStart = 12;
    public const int TextureCharsPerLine = 14;
    public const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.?!,$:";

    public static void Draw(string text, int x, int y, TextureAtlas atlas, SpriteBatch batch, Camera camera, bool withBackground = true, float scale = 1f)
    {
        var scaleVec = new Vector2(scale);
        
        text = text.ToUpper();

        if (withBackground)
        {
            var size = new Vector2((text.Length + 1f) * scale, scale * 2f);
            var pos = new Vector2(x + (size.X * 0.5f - scale) * atlas.TileSize, y);
            atlas.Draw(batch, camera, pos, 8, 11, 1, 1, Color.White, size);
        }
        
        var i = 0;
        foreach (char c in text)
        {
            if (c != ' ')
            {
                int index = Characters.IndexOf(c);
                int texX = TextureXStart + index % TextureCharsPerLine;
                int texY = TextureYStart + index / TextureCharsPerLine;
                atlas.Draw(batch, camera, new Vector2(x + atlas.TileSize * scale * i, y), texX, texY, 1, 1, Color.White, scaleVec);
            }

            i++;
        }
    }
}