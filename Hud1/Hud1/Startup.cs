using Hud1.Helpers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
namespace Hud1
{
    class Startup
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
            Debug.Print("RunApp {0}", Hud1.Startup.Millis());
            App app = new();
            Debug.Print("A {0}", Hud1.Startup.Millis());
            app.Run();
        }

        public static long Millis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startMS;
        }
    }
}
