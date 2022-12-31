using System.Text.Json.Serialization;

namespace Shared;

public class TileData
{
    [JsonInclude] public int TextureIndex;
    [JsonInclude] public bool AutoTile;
    [JsonInclude] public bool Solid;
    [JsonInclude] public int Effect;
}