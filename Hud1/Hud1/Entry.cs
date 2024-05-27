using System.Diagnostics;

namespace Hud1;

internal class Entry
{
    private static readonly long _startMS = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    [STAThread]
    public static void Main(string[] args)
    {
        RunApp();
    }

    private static void RunApp()
    {
        Debug.Print("Entry RunApp {0}", Hud1.Entry.Millis());
        App app = new();
        app.Run();
    }

    public static long Millis()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startMS;
    }
}
