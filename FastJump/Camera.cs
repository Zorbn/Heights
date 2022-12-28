using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FastJump;

public class Camera
{
    public readonly bool Cull;
    private Vector2 offset;

    public float Scale { get; private set; }

    public Vector2 Position { get; private set; }

    public int VirtualViewWidth { get; }
    public int VirtualViewHeight { get; }
    public int ViewWidth { get; private set; }
    public int ViewHeight { get; private set; }

    public Camera(int virtualViewWidth, int virtualViewHeight, bool cull = true)
    {
        VirtualViewWidth = virtualViewWidth;
        VirtualViewHeight = virtualViewHeight;
        ViewWidth = VirtualViewWidth;
        ViewHeight = VirtualViewHeight;
        Cull = cull;
        Scale = 1f;

        UpdateOffset();
    }
    
    public void GoTo(Vector2 target)
    {
        Vector2 newPos = target - offset;
        Position = newPos;
    }

    public void ScaleToScreen(int width, int height)
    {
        Scale = MathF.Min(width / (float)VirtualViewWidth, height / (float)VirtualViewHeight);
        ViewWidth = (int)(width / Scale);
        ViewHeight = (int)(height / Scale);
        UpdateOffset();
    }

    public Vector2 GetMousePosition()
    {
        MouseState mouseState = Mouse.GetState();
        return new Vector2(mouseState.X / Scale + Position.X, mouseState.Y / Scale + Position.Y);
    }

    private void UpdateOffset()
    {
        offset.X = ViewWidth * 0.5f;
        offset.Y = ViewHeight * 0.5f;
    }
}