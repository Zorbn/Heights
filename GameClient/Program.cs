using System;

namespace GameClient;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        using var game = new GameClient();
        game.Run();
    }
}