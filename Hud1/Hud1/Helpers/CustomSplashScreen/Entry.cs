using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Services.Store;
using Windows.Storage;

namespace Hud1.Helpers.CustomSplashScreen
{
    class DebugWriter : TextWriter
    {
        private string Path;

        public DebugWriter(string path)
        {
            Path = path;
            if (File.Exists(Path))
                File.Delete(Path);
        }

        public override void WriteLine(string? value)
        {
            Debug.WriteLine(value);
            using var fileStream = new FileStream(Path, FileMode.Append);
            using var streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(value);
        }

        public override void Write(string? value)
        {
            Debug.Write(value);
            using var fileStream = new FileStream(Path, FileMode.Append);
            using var streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(value);
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

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Console.WriteLine("Copy {0}, {1}", sourcePath, targetPath);
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                RootPath = storageFolder.Path;

                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;
                var Version = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

                VersionPath = Path.Combine(RootPath, Version);
            }
            catch (Exception e)
            {
                RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Local");
                if (!Directory.Exists(RootPath))
                {
                    Directory.CreateDirectory(RootPath);
                }
                VersionPath = Path.Combine(RootPath, "0.0.0.0");
            }

            var writer = new DebugWriter(Path.Combine(RootPath, "log.txt"));
            Console.SetOut(writer);
            Console.SetError(writer);

            Console.WriteLine("=============================================================");
            Console.WriteLine("=== RootPath {0}", RootPath);
            Console.WriteLine("=== VersionPath {0}", VersionPath);

            try
            {
                if (!Directory.Exists(VersionPath))
                {
                    Console.WriteLine("=== Creating New Version");
                    Directory.CreateDirectory(VersionPath);

                    Directory.CreateDirectory(Path.Combine(VersionPath, "Macros"));
                    Directory.CreateDirectory(Path.Combine(VersionPath, "Fonts"));

                    CopyFilesRecursively(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros"),
                        Path.Combine(VersionPath, "Macros")
                        );
                    CopyFilesRecursively(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts"),
                        Path.Combine(VersionPath, "Fonts")
                        );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating version");
                Console.WriteLine(e);
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

        public async static void CheckLicense()
        {
            Console.WriteLine("Check License...");
            var context = StoreContext.GetDefault();
            var appLicense = await context.GetAppLicenseAsync();

            Console.WriteLine("Check License... IsActive {0}", appLicense.IsActive);
            Console.WriteLine("Check License... IsTrial {0}", appLicense.IsTrial);
            Console.WriteLine("Check License... ExpirationDate {0}", appLicense.ExpirationDate);
        }

        private static void PumpDispatcherUntilPriority(DispatcherPriority dispatcherPriority)
        {
            var dispatcherFrame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke((ThreadStart)(() => dispatcherFrame.Continue = false), dispatcherPriority);
            Dispatcher.PushFrame(dispatcherFrame);
        }
    }
}
