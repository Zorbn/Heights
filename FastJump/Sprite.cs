using Microsoft.Xna.Framework;

namespace FastJump;

public class Sprite
{
    public Vector2 Position { get; private set; }

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
}