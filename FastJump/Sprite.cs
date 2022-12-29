using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Shared;

namespace FastJump;

public class Sprite
{
    private static readonly Dictionary<Animation, AnimationData> Animations = new();
    
    public Vector2 Position { get; private set; }
    private Animation currentAnimation = Animation.PlayerIdle;
    private float animationTime;

    public Sprite(Vector2 position)
    {
        Position = position;
    }

    public void Teleport(Vector2 position)
    {
        Position = position;
    }

    public void StepTowards(Vector2 position, float delta)
    {
        Vector2 newPosition = Position;
        newPosition.X = Position.X + (position.X - Position.X) * delta;
        newPosition.Y = Position.Y + (position.Y - Position.Y) * delta;
        Position = newPosition;
    }

    public Frame UpdateAnimation(Animation animation, float deltaTime)
    {
        if (animation != currentAnimation)
        {
            currentAnimation = animation;
            animationTime = 0;
        }

        AnimationData animationData = Animations[currentAnimation];
        animationTime += deltaTime * animationData.Speed;
        int frameIndex;
        if (animationData.Loop)
        {
            frameIndex = (int)animationTime % animationData.Frames.Length;
        }
        else
        {
            frameIndex = Math.Min((int)animationTime, animationData.Frames.Length - 1);
        }
        
        return animationData.Frames[frameIndex];
    }

    public static void LoadAnimation(Animation target, string path)
    {
        string text = File.ReadAllText(path);
        object dataObj = JsonSerializer.Deserialize(text, typeof(AnimationData));

        if (dataObj is not AnimationData newAnimationData) throw new ArgumentException("Failed to load animation json!");
        
        Animations.Add(target, newAnimationData);
    }
}