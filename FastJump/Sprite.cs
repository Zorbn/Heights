using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Shared;

namespace FastJump;

public class Sprite
{
    private const float AnimationSpeed = 8f;
    private static readonly Dictionary<Animation, Frame[]> Animations = new()
    {
        { Animation.PlayerIdle, new Frame[]{ new() {X = 0, Y = 0} } },
        { 
            Animation.PlayerRunning, new Frame[]
            {
                new() {X = 0, Y = 0},
                new() {X = 0, Y = 2},
                new() {X = 0, Y = 4},
                new() {X = 0, Y = 6},
                new() {X = 0, Y = 8},
                new() {X = 0, Y = 10},
                new() {X = 0, Y = 12},
                new() {X = 0, Y = 14},
            } 
        },
    };
    
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

        animationTime += deltaTime * AnimationSpeed;
        int frameIndex = (int)animationTime % Animations[currentAnimation].Length;
        return Animations[currentAnimation][frameIndex];
    }
}