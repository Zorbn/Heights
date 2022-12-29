using System.Text.Json.Serialization;

namespace FastJump;

public struct Frame
{
    [JsonInclude] public int X;
    [JsonInclude] public int Y;
}