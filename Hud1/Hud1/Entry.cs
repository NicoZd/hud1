using Hud1.Helpers;
using System.Diagnostics;

namespace Hud1;

internal class Entry
{
    private static readonly long startMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    [STAThread]
    public static void Main(string[] args)
    {
        var writer = new DebugWriter();
        Console.SetOut(writer);
        Console.SetError(writer);

        RunApp();
    }

    internal static long Millis()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() - startMs;
    }

    private static void RunApp()
    {
        Debug.Print("Entry RunApp {0}", Entry.Millis());
        App app = new();
        app.Run();
    }
}
