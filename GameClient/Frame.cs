using System.Text.Json.Serialization;

namespace GameClient;

public struct Frame
{
    [JsonInclude] public int X;
    [JsonInclude] public int Y;
}