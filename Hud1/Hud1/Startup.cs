using Hud1.Helpers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Windows.ApplicationModel;
using Windows.Services.Store;
using Windows.Storage;

namespace Hud1
{
    class Startup
    {
        public static readonly uint WM_GAME_DIRECT_SHOWME = WindowsAPI.RegisterWindowMessage("WM_GAME_DIRECT_SHOWME");

        static Mutex mutex = new Mutex(true, "GAME_DIRECT");

        public static string RootPath = "";
        public static string VersionPath = "";

        public static async Task Run()
        {
            await ShowSplash("Compute Paths");
            ComputePaths();

            await ShowSplash("Create Root");
            CreateRootPath();

            await ShowSplash("Setup Logging");
            SetupLogging();
            Console.WriteLine("Startup Log starts at {0} {1}", DateTime.Now, Entry.Millis());

            await ShowSplash("Check Version");
            LazyCreateVersion();

            await ShowSplash("Single Instance");
            await EnforceSingleInstance();

            await ShowSplash("Check License");
            await CheckLicense();

            await ShowSplash("Complete");
        }

        private static async Task ShowSplash(string text)
        {
            Console.WriteLine($"Startup ShowSplash: {text} {Entry.Millis()}");
            SplashWindow.Instance!.SplashText = text;
            await Task.Delay(TimeSpan.FromMilliseconds(20));
        }

        private static async Task EnforceSingleInstance()
        {
            try
            {
                int startRount = 0;
                while (!mutex.WaitOne(TimeSpan.Zero, true) && startRount <= 10)
                {
                    await ShowSplash("Startup Shutdown existing window (attempt: " + startRount + "/10)");
                    WindowsAPI.SendNotifyMessage(new nint(-1), WM_GAME_DIRECT_SHOWME, 0, 0);
                    await Task.Delay(500);
                    startRount++;
                }
            }
            catch (AbandonedMutexException)
            {
            }

            // never every start second time
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                await ShowSplash("Could not stop existing window - shutting down.. :(");
                await Task.Delay(1000);
                SplashWindow.Instance!.Close();
            }
        }

        private static void SetupLogging()
        {
            // configure logging
            var writer = new DebugWriter(Path.Combine(RootPath, "log.txt"));
            Console.SetOut(writer);
            Console.SetError(writer);
        }

        private static void CreateRootPath()
        {
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
        }

        private static void LazyCreateVersion()
        {
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
                Directory.Delete(VersionPath, true);

                throw new Exception("Could not create version", e);
            }
        }

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

        private static void ComputePaths()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                RootPath = Path.Combine(storageFolder.Path, "Game Direct");

                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;
                var Version = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
                VersionPath = Path.Combine(RootPath, Version);
            }
            catch (Exception)
            {
                RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Direct");
                VersionPath = Path.Combine(RootPath, "0.0.0.0");
            }
        }

        private static async Task CheckLicense()
        {
            Console.WriteLine("Startup Check License... {0}", Entry.Millis());

            var context = StoreContext.GetDefault();
            var appLicense = await context.GetAppLicenseAsync();

            Console.WriteLine("Startup Check License... IsActive {0}", appLicense.IsActive);
            Console.WriteLine("Startup Check License... IsTrial {0}", appLicense.IsTrial);
            Console.WriteLine("Startup Check License... ExpirationDate {0}", appLicense.ExpirationDate);

            Console.WriteLine("Startup Check License Complete {0}", Entry.Millis());

            if (!appLicense.IsActive)
            {
                SplashWindow.Instance!.Close();
                if (MessageBox.Show("App License is inactive. Unfortunately the application must shutdown. Do you want to open the App in the Microsoft Store?", "Game Direct", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ms-windows-store://pdp/?productid=9NCQ2311M9XV",
                        UseShellExecute = true
                    });
                }
            }
        }
    }
}
