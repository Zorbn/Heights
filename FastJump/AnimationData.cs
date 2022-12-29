using System.Text.Json.Serialization;

namespace FastJump;

public class AnimationData
{
    [JsonInclude] public float Speed;
    [JsonInclude] public bool Loop;
    [JsonInclude] public Frame[] Frames;
}