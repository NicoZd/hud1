using System.Diagnostics;

namespace Hud1
{
    class Entry
    {
        private static long _startMS;

        [STAThread]
        public static void Main(string[] args)
        {
            _startMS = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
}
