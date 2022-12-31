using Microsoft.Xna.Framework.Graphics;

namespace GameClient;

public interface IGameState
{
    public void Initialize(GameClient gameClient, int screenWidth, int screenHeight, params string[] args);
    public void Update(Input input, float deltaTime);
    public void Draw(Background background, TextureAtlas atlas, SpriteBatch batch, int windowWidth, int windowHeight, float deltaTime);
    public void Dispose();
}