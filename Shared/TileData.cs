using System.Text.Json.Serialization;

namespace Shared;

public struct TileData
{
    [JsonInclude] public int TextureIndex;
    [JsonInclude] public bool AutoTile;
    [JsonInclude] public bool Solid;
}