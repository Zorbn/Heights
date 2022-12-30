using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Shared;

public class MapData
{
    [JsonInclude] public char[] Data;
    [JsonInclude] public int Height;
    [JsonInclude] public Dictionary<char, TileData> Palette;
    public Vector2 SpawnPos;
    [JsonInclude] public int TileSize;
    [JsonInclude] public int Width;

    public MapData()
    {
        Palette = new Dictionary<char, TileData>();
        Data = new char[Width * Height];
    }

    public static MapData LoadFromFile(string path)
    {
        string text = File.ReadAllText(path);
        object dataObj = JsonSerializer.Deserialize(text, typeof(MapData));

        if (dataObj is not MapData newMapData) throw new ArgumentException("Failed to load map json!");

        return newMapData;
    }

    public char GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return ' ';
        return Data[x + y * Width];
    }

    public char GetTileAtWorldPos(float x, float y)
    {
        var tileX = (int)MathF.Floor(x / TileSize);
        var tileY = (int)MathF.Floor(y / TileSize);
        return GetTile(tileX, tileY);
    }

    public bool IsCollidingWith(Vector2 position, Vector2 size)
    {
        for (var i = 0; i < 4; i++)
        {
            int xOff = i % 2;
            int yOff = i / 2;

            float xDir = xOff - 0.5f;
            float yDir = yOff - 0.5f;

            float cornerX = position.X + xDir * size.X;
            float cornerY = position.Y + yDir * size.Y;
            char tile = GetTileAtWorldPos(cornerX, cornerY);
            if (Palette[tile].Solid) return true;
        }

        return false;
    }

    public void FindSpawnPos()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            if (GetTile(x, y) != '[') continue;
            SpawnPos = new Vector2((x + 0.5f) * TileSize, (y + 0.5f) * TileSize);
            break;
        }
    }
}