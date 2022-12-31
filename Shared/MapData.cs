using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Shared;

public class MapData
{
    [JsonInclude] public int TileSize;
    [JsonInclude] public int Width;
    [JsonInclude] public int Height;
    [JsonInclude] public Dictionary<string, int> Effect;
    [JsonInclude] public Dictionary<char, TileData> Palette;
    [JsonInclude] public string[] Data;

    public Vector2 SpawnPos;

    public static MapData LoadFromFile(string path)
    {
        string text = File.ReadAllText(path);
        object dataObj = JsonSerializer.Deserialize(text, typeof(MapData));

        if (dataObj is not MapData newMapData) throw new ArgumentException("Failed to load map json!");
        
        // Validate that the size of the map is correct.
        if (newMapData.Data.Length != newMapData.Height)
            throw new ArgumentException(
                $"Map should have {newMapData.Height} rows, but has {newMapData.Data.Length} rows instead!");
        
        for (var i = 0; i < newMapData.Data.Length; i++)
        {
            int length = newMapData.Data[i].Length;
            if (length != newMapData.Width) throw new ArgumentException(
                $"Map should have {newMapData.Width} columns, but has {length} columns instead on row {i}!");
        }
        
        return newMapData;
    }

    public char GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return ' ';
        return Data[y][x];
    }
    
    public TileData GetTileData(int x, int y)
    {
        return Palette[GetTile(x, y)];
    }
    
    public int GetTilePos(float p)
    {
        return (int)MathF.Floor(p / TileSize);
    }

    public char GetTileAtWorldPos(Vector2 pos)
    {
        int tileX = GetTilePos(pos.X);
        int tileY = GetTilePos(pos.Y);
        return GetTile(tileX, tileY);
    }
    
    public TileData GetTileDataAtWorldPos(Vector2 pos)
    {
        int tileX = GetTilePos(pos.X);
        int tileY = GetTilePos(pos.Y);
        return GetTileData(tileX, tileY);
    }

    public bool IsCollidingWith(Vector2 position, Vector2 size)
    {
        for (var i = 0; i < 4; i++)
        {
            int xOff = i % 2;
            int yOff = i / 2;

            float xDir = xOff - 0.5f;
            float yDir = yOff - 0.5f;

            var corner = new Vector2(position.X + xDir * size.X, position.Y + yDir * size.Y);
            char tile = GetTileAtWorldPos(corner);
            if (Palette[tile].Solid) return true;
        }

        return false;
    }

    public void FindSpawnPoint()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            if (GetTileData(x, y).Effect != Effect["Spawn"]) continue;
            var center = new Vector2((x + 0.5f) * TileSize, (y + 0.5f) * TileSize);
            SpawnPos = center;
        }
    }
}