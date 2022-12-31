using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared;

namespace GameClient;

public class Map
{
    public readonly MapData MapData;

    public Map(string path)
    {
        MapData = MapData.LoadFromFile(path);
    }

    public void Draw(TextureAtlas atlas, SpriteBatch batch, Camera camera)
    {
        int minX = Math.Max(MapData.GetTilePos(camera.Position.X) - 1, 0);
        int maxX = Math.Min(MapData.GetTilePos(camera.Position.X + camera.ViewWidth) + 1, MapData.Width);
        int minY = Math.Max(MapData.GetTilePos(camera.Position.Y) - 1, 0);
        int maxY = Math.Min(MapData.GetTilePos(camera.Position.Y + camera.ViewHeight) + 1, MapData.Height);

        for (int x = minX; x < maxX; x++)
        for (int y = minY; y < maxY; y++)
        {
            char currentTile = MapData.GetTile(x, y);
            if (currentTile == ' ') continue;

            int atlasX = MapData.Palette[currentTile].TextureIndex % atlas.Width;
            int atlasY = MapData.Palette[currentTile].TextureIndex / atlas.Width;

            if (!MapData.Palette[currentTile].AutoTile)
            {
                var tilePos = new Vector2((x + 0.5f) * MapData.TileSize,
                    (y + 0.5f) * MapData.TileSize);
                atlas.Draw(batch, camera, tilePos, atlasX, atlasY, 2, 2, Color.White);

                continue;
            }

            for (var i = 0; i < 4; i++)
            {
                int xOff = i % 2;
                int yOff = i / 2;

                int xDir = xOff * 2 - 1;
                int yDir = yOff * 2 - 1;

                var subOffX = 1;
                var subOffY = 1;

                if (MapData.GetTile(x + xDir, y) != currentTile) subOffX += xDir;
                if (MapData.GetTile(x, y + yDir) != currentTile) subOffY += yDir;

                if (subOffX == 1 && subOffY == 1 && MapData.GetTile(x + xDir, y + yDir) != currentTile)
                {
                    subOffX = 4 - xOff;
                    subOffY = 1 - yOff;
                }

                var subTilePos = new Vector2((x + 0.5f + 0.5f * xOff) * MapData.TileSize,
                    (y + 0.5f + 0.5f * yOff) * MapData.TileSize);
                atlas.Draw(batch, camera, subTilePos, atlasX + subOffX, atlasY + subOffY, 1, 1, Color.White);
            }
        }
    }
}