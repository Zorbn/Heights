using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FastJump;

public class TextRenderer
{
    public const int TextureXStart = 2;
    public const int TextureYStart = 12;
    public const int TextureCharsPerLine = 14;
    public const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.?!,$";

    public static void Draw(string text, int x, int y, TextureAtlas atlas, SpriteBatch batch, Camera camera)
    {
        text = text.ToUpper();
        
        var i = 0;
        foreach (char c in text)
        {
            if (c != ' ')
            {
                int index = Characters.IndexOf(c);
                int texX = TextureXStart + index % TextureCharsPerLine;
                int texY = TextureYStart + index / TextureCharsPerLine;
                atlas.Draw(batch, camera, new Vector2(x + atlas.TileSize * i, y), texX, texY, 1, 1, Color.White);
            }

            i++;
        }
    }
}