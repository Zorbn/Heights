using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared;

namespace FastJump;

public class Map
{
    public readonly MapData MapData;

    public Map(string path)
    {
        MapData = MapData.LoadFromFile(path);
    }

    public void Draw(TextureAtlas atlas, SpriteBatch batch, Camera camera)
    {
        for (var y = 0; y < MapData.Height; y++)
        {
            for (var x = 0; x < MapData.Width; x++)
            {
                char currentTile = MapData.Data[x + y * MapData.Width];
                if (currentTile == ' ') continue;

                int atlasIndex = MapData.Palette[currentTile].TextureIndex;

                if (!MapData.Palette[currentTile].AutoTile)
                {
                    var tilePos = new Vector2((x + 0.5f) * MapData.TileSize,
                        (y + 0.5f) * MapData.TileSize);
                    atlas.Draw(batch, camera, tilePos, atlasIndex, 0, 2, 2, Color.White);
                    
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
                    atlas.Draw(batch, camera, subTilePos, atlasIndex + subOffX, subOffY, 1, 1, Color.White);
                }
            }
        }
    }
}