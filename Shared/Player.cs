using Microsoft.Xna.Framework;

namespace Shared;

public class Player
{
    private static readonly Vector2 HitBoxSize = new(8f, 14f);
    private const float JumpForce = 1.5f;
    private const float ExtraHeightGravityMultiplier = 0.5f;
    private const float Speed = 100f;
 
    public Vector2 Position;
    public Direction Direction = Direction.Right;

    private float velocity;
    private bool grounded;
    private bool extraHeight;
    
    public Player(Vector2 position)
    {
        Position = position;
    }

    public void Move(float horizontalDir, bool tryJump, bool noClip, MapData mapData, float deltaTime)
    {
        Vector2 newPosition = Position;
        Vector2 move = Vector2.Zero;
        
        move.X = horizontalDir;

        if (move.X > 0f)
        {
            Direction = Direction.Right;
        }
        else if (move.X < 0f)
        {
            Direction = Direction.Left;
        }
        
        newPosition.X += move.X * Speed * deltaTime;
        
        if (!noClip && mapData.IsCollidingWith(newPosition, HitBoxSize))
        {
            newPosition.X = GetMaxPosInTile(Position.X, HitBoxSize.X, move.X, mapData.TileSize);
        }

        // Make holding down the jump button after jumping make the player jump higher.
        float gravityMultiplier = extraHeight && velocity < 0f ? ExtraHeightGravityMultiplier : 1.0f;
        velocity += gravityMultiplier * Physics.Gravity * deltaTime;

        if (tryJump)
        {
            if (grounded)
            {
                extraHeight = true;
                velocity = -JumpForce;
            }
        }
        else
        {
            extraHeight = false;
        }

        move.Y = velocity;

        newPosition.Y += move.Y * Speed * deltaTime;
        grounded = false;
        
        if (!noClip && mapData.IsCollidingWith(newPosition, HitBoxSize))
        {
            if (velocity > 0f)
            {
                grounded = true;
            }

            newPosition.Y = GetMaxPosInTile(Position.Y, HitBoxSize.Y, move.Y, mapData.TileSize);
            velocity = 0f;
        }

        Position = newPosition;
    }

    // Find a position that would press the player up against the next tile.
    private float GetMaxPosInTile(float pos, float size, float direction, int tileSize)
    {
        if (direction > 0f)
        {
            return MathF.Ceiling(pos / tileSize) * tileSize - size * 0.51f;
        }

        return MathF.Floor(pos / tileSize) * tileSize + size * 0.51f;
    }
}