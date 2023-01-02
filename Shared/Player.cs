using Microsoft.Xna.Framework;

namespace Shared;

public class Player
{
    public const int StartingScore = 1000;
    public const string DefaultName = "Unnamed";
    public const int MaxNameLength = 13;
    public const float NameScale = 0.5f;
    
    private const float JumpForce = 1.5f;
    private const float JumpPadForceMultiplier = 4f;
    private const float ExtraHeightGravityMultiplier = 0.5f;
    private const float Speed = 100f;
    private static readonly Vector2 HitBoxSize = new(8f, 14f);
    public Animation Animation = Animation.PlayerIdle;
    public Direction Direction = Direction.Right;
    private bool extraHeight;
    private bool grounded;

    public Vector2 Position;
    public int Score;
    public int HighScore;
    public string Name = DefaultName;

    private float velocity;

    public Player(Vector2 position, string name, int highScore)
    {
        Position = position;
        Name = name;
        HighScore = highScore;
    }

    public void Move(float horizontalDir, bool tryJump, MapData mapData, float deltaTime)
    {
        TileData tileAtPlayer = mapData.GetTileDataAtWorldPos(Position);

        Vector2 newPosition = Position;
        Vector2 move = Vector2.Zero;
        
        move.X = horizontalDir;

        Direction = move.X switch
        {
            > 0f => Direction.Right,
            < 0f => Direction.Left,
            _ => Direction
        };

        if (grounded)
            Animation = move.X != 0f ? Animation.PlayerRunning : Animation.PlayerIdle;
        else
            Animation = Animation.PlayerJumping;

        newPosition.X += move.X * Speed * deltaTime;

        if (mapData.IsCollidingWith(newPosition, HitBoxSize))
            newPosition.X = GetMaxPosInTile(Position.X, HitBoxSize.X, move.X, mapData.TileSize);

        // Make holding down the jump button after jumping make the player jump higher.
        float gravityMultiplier = extraHeight && velocity < 0f ? ExtraHeightGravityMultiplier : 1.0f;
        velocity += gravityMultiplier * Physics.Gravity * deltaTime;

        if (tileAtPlayer.Effect == mapData.Effect["Jump"] && grounded)
        {
            Audio.PlaySoundWithPitch(Sound.Bounce);
            velocity = -JumpForce * JumpPadForceMultiplier;
        }
        
        if (tryJump)
        {
            if (grounded)
            {
                Audio.PlaySoundWithPitch(Sound.Jump);
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
        grounded = velocity > 0f && mapData.IsCollidingWith(Position + new Vector2(0f, 1f), HitBoxSize);

        if (mapData.IsCollidingWith(newPosition, HitBoxSize))
        {
            newPosition.Y = GetMaxPosInTile(Position.Y, HitBoxSize.Y, move.Y, mapData.TileSize);
            velocity = 0f;
        }

        Position = newPosition;
    }

    // Find a position that would press the player up against the next tile.
    private float GetMaxPosInTile(float pos, float size, float direction, int tileSize)
    {
        // Slightly more than 1/2 so that the player is placed fully outside of the tile's hit-box.
        const float sizeMultiplier = 0.501f;
        
        if (direction > 0f) return MathF.Ceiling(pos / tileSize) * tileSize - size * sizeMultiplier;

        return MathF.Floor(pos / tileSize) * tileSize + size * sizeMultiplier;
    }
}