using System.Text.Json.Serialization;

namespace GameClient;

public class AnimationData
{
    [JsonInclude] public Frame[] Frames;
    [JsonInclude] public bool Loop;
    [JsonInclude] public float Speed;
}