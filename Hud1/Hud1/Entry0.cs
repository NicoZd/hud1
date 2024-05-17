using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using Windows.ApplicationModel;

using Windows.Foundation.Metadata;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Services.Store;
using Windows.Storage;
using WpfScreenHelper;

namespace Hud1
{
    class Entry0
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
            Debug.Print("RunApp {0}", Hud1.Entry0.Millis());

            App app = new();
            Debug.Print("A {0}", Hud1.Entry0.Millis());
            app.Run();
        }

        public static long Millis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startMS;
        }
    }
}
