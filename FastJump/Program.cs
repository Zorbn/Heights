using System;

namespace FastJump;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        using var game = new FastJumpGame();
        game.Run();
    }
}