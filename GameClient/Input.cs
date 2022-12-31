using Microsoft.Xna.Framework.Input;

namespace GameClient;

public class Input
{
    public KeyboardState PreviousState { get; private set; }
    public KeyboardState CurrentState { get; private set; }

    public Input()
    {
        CurrentState = Keyboard.GetState();
        PreviousState = CurrentState;
    }

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key)
    {
        return CurrentState.IsKeyDown(key);
    }

    public bool WasKeyPressed(Keys key)
    {
        return CurrentState.IsKeyDown(key) && !PreviousState.IsKeyDown(key);
    }
}