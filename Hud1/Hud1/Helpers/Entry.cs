using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
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

namespace Hud1.Helpers
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

        public static readonly uint WM_GAME_DIRECT_SHOWME = WindowsAPI.RegisterWindowMessage("WM_GAME_DIRECT_SHOWME");

        static Mutex mutex = new Mutex(true, "GAME_DIRECT");

        public static string RootPath = "";
        public static string VersionPath = "";

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

        public static void SendShutdown()
        {
            WindowsAPI.SendNotifyMessage(new nint(-1), WM_GAME_DIRECT_SHOWME, 0, 0);
        }


        public static void Main()
        {
            // try close exising apps
            try
            {
                int startRount = 0;
                while (!mutex.WaitOne(TimeSpan.Zero, true) && startRount < 10)
                {
                    Debug.Print("SendShutdown {0}", startRount);
                    SendShutdown();
                    Thread.Sleep(500);
                    startRount++;
                }
            }
            catch (AbandonedMutexException)
            {
            }

            // never every start second time
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Debug.Print("Refuse to start...");
                return;
            }

            InitPaths();

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

        }

        public static void InitPaths()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                RootPath = Path.Combine(storageFolder.Path, "Game Direct");
                if (!Directory.Exists(RootPath))
                {
                    Directory.CreateDirectory(RootPath);
                }

                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;
                var Version = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

                VersionPath = Path.Combine(RootPath, Version);
            }
            catch (Exception)
            {
                RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Direct");
                if (!Directory.Exists(RootPath))
                {
                    Directory.CreateDirectory(RootPath);
                }
                VersionPath = Path.Combine(RootPath, "0.0.0.0");
            }
        }

        public async static void CheckLicense()
        {
            Console.WriteLine("Check License...");
            var context = StoreContext.GetDefault();
            var appLicense = await context.GetAppLicenseAsync();

            Console.WriteLine("Check License... IsActive {0}", appLicense.IsActive);
            Console.WriteLine("Check License... IsTrial {0}", appLicense.IsTrial);
            Console.WriteLine("Check License... ExpirationDate {0}", appLicense.ExpirationDate);

            if (!appLicense.IsActive)
            {
                if (MessageBox.Show("App License is inactive. Unfortunately the application must shutdown. Do you want to open the App in the Microsoft Store?", "Game Direct", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ms-windows-store://pdp/?productid=9NCQ2311M9XV",
                        UseShellExecute = true
                    });
                }
                Application.Current.Shutdown();
            }
        }

        private static void PumpDispatcherUntilPriority(DispatcherPriority dispatcherPriority)
        {
            var dispatcherFrame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke((ThreadStart)(() => dispatcherFrame.Continue = false), dispatcherPriority);
            Dispatcher.PushFrame(dispatcherFrame);
        }
    }
}
