using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Threading;
using Windows.ApplicationModel;
using Windows.Storage;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;

namespace Hud1.Helpers.CustomSplashScreen
{
    class DebugWriter : TextWriter
    {
        internal StreamWriter streamwriter;
        private string v;

        public DebugWriter(string path)
        {
            FileStream filestream = new FileStream(path, FileMode.Create);
            streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
        }

        public override void WriteLine(string? value)
        {
            Debug.WriteLine(value);
            streamwriter.WriteLine(value);
        }

        public override void Write(string? value)
        {
            Debug.Write(value);
            streamwriter.Write(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }

    class Entry
    {
        public static string RootPath = "";
        public static string VersionPath = "";

        private static readonly SplashScreenWrapper splashScreen = new(resourceName: "/Assets/ai-generated-8641785-splash.png");

        private static readonly App app = new();

        [STAThread]
        public static void Main(string[] args)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            RootPath = storageFolder.Path;

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            var Version = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            VersionPath = Path.Combine(RootPath, Version);

            var writer = new DebugWriter(Path.Combine(RootPath, "log.txt"));
            Console.SetOut(writer);
            Console.SetError(writer);

            Debug.Print("Debug");
            Console.WriteLine("==============Console {0}", RootPath);
            Console.WriteLine("==============Console {0}", VersionPath);

            try
            {
                if (!Directory.Exists(VersionPath))
                {
                    Console.WriteLine("==============Creating Version", RootPath);
                    Directory.CreateDirectory(VersionPath);
                    // copy all stuff
                }
            }
            catch (Exception ex)
            {
                Directory.Delete(VersionPath);
            }

            var showSplash = !args.Any(x => string.Equals("-nosplash", x, StringComparison.OrdinalIgnoreCase));

            if (showSplash)
            {
                splashScreen.Show(autoClose: false);
            }

            app.InitializeComponent();

            if (showSplash)
            {
                // pump until loaded
                PumpDispatcherUntilPriority(DispatcherPriority.Loaded);

                // start a timer, after which the splash can be closed
                var splashTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(0.1)
                };
                splashTimer.Tick += (s, e) =>
                {
                    splashTimer.Stop();
                    splashScreen.Close(TimeSpan.FromMilliseconds(150));
                };
                splashTimer.Start();
            }

            app.Run();
        }
        private static void PumpDispatcherUntilPriority(DispatcherPriority dispatcherPriority)
        {
            var dispatcherFrame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke((ThreadStart)(() => dispatcherFrame.Continue = false), dispatcherPriority);
            Dispatcher.PushFrame(dispatcherFrame);
        }
    }
}
