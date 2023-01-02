using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient;

public class TextRenderer
{
    public const int TextureXStart = 2;
    public const int TextureYStart = 12;
    public const int TextureCharsPerLine = 14;
    public const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.?!,$:";

    public static void Draw(string text, int x, int y, TextureAtlas atlas, SpriteBatch batch, Camera camera, bool withBackground = true, float scale = 1f, bool centered = false)
    {
        var scaleVec = new Vector2(scale);
        var size = new Vector2((text.Length + 1f) * scale, scale * 2f);
        float textOffsetX = centered ? (size.X * 0.5f - scale) * -atlas.TileSize : 0f;

        text = text.ToUpper();

        if (withBackground)
        {
            float bgOffsetX = centered ? 0f : (size.X * 0.5f - scale) * atlas.TileSize;
            var pos = new Vector2(x + bgOffsetX, y);
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
                atlas.Draw(batch, camera, new Vector2(x + textOffsetX + atlas.TileSize * scale * i, y), texX, texY, 1, 1, Color.White, scaleVec);
            }

            i++;
        }
    }
}