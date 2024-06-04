using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using Windows.ApplicationModel;
using Windows.Services.Store;
using Windows.Storage;

namespace Hud1;

public class Setup
{
    internal static readonly uint WM_GAME_DIRECT_SHOWME = WindowsAPI.RegisterWindowMessage("WM_GAME_DIRECT_SHOWME");

    private static readonly Mutex? Mutex = new(true, "GAME_DIRECT");

    public static string RootPath = "";
    public static string VersionPath = "";
    public static string UserConfigFile = "";

    public static async Task Run()
    {
        await ShowSplash("Compute Paths");
        ComputePaths();

        await ShowSplash("Create Root");
        CreateRootPath();

        await ShowSplash("Setup Logging");
        SetupLogging();
        Console.WriteLine($"Setup Logfile starts {DateTime.Now} {Entry.Millis()}");

        await ShowSplash("Single Instance");
        await EnforceSingleInstance();

        await ShowSplash("Check License");
        await CheckLicense();

        await ShowSplash("Check Version");
        LazyCreateVersion();

        await ShowSplash("Apply Config");
        ApplyConfig();

        await ShowSplash("Complete");
    }

    private static void ApplyConfig()
    {
        try
        {
            if (File.Exists(UserConfigFile))
            {
                var userConfigString = File.ReadAllText(UserConfigFile);
                // Console.WriteLine("UserConfigString {0}", userConfigString);
                var loaded = JsonSerializer.Deserialize<UserConfig>(userConfigString);

                var config = UserConfig.Current;
                Console.Write("Apply User Config:");
                foreach (var prop in config.GetType().GetProperties())
                {
                    var value = prop.GetValue(loaded, null);
                    if (value != null)
                    {
                        Console.WriteLine("\t{0} = {1},", prop.Name, value);
                        prop.SetValue(config, value);
                    }
                }
            }
            else
                Console.WriteLine("No UserConfig File {0}", UserConfigFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Loading UserConfig:");
            Console.WriteLine(ex.ToString());
        }

        // add to navgation
        NightvisionViewModel.Instance.BuildNavigation();
        CrosshairViewModel.Instance.BuildNavigation();
        MacrosViewModel.Instance.BuildNavigation();
        MoreViewModel.Instance.BuildNavigation();

        // update
        HudViewModel.Instance.BuildNavigation();

        var showGraph = false;
        if (showGraph)
        {
            HudViewModel.Instance.ShowGraph();
        }

        // add change listeners

        // Nightvision
        NightvisionViewModel.Instance.PropertyChanged += OnConfigChanged(
            nameof(NightvisionViewModel.Instance.GammaIndex),
            nameof(UserConfig.Current.GammaIndex));

        // Chrosshair
        NavigationStates.CROSSHAIR_ENABLED.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean),
            nameof(UserConfig.Current.CrosshairEnabled));

        NavigationStates.CROSSHAIR_DISPLAY.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel),
            nameof(UserConfig.Current.CrosshairDisplay));

        NavigationStates.CROSSHAIR_FORM.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_FORM.SelectionLabel),
            nameof(UserConfig.Current.CrosshairForm));

        NavigationStates.CROSSHAIR_COLOR.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_COLOR.SelectionLabel),
            nameof(UserConfig.Current.CrosshairColor));

        NavigationStates.CROSSHAIR_OPACITY.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_OPACITY.SelectionLabel),
            nameof(UserConfig.Current.CrosshairOpacity));

        NavigationStates.CROSSHAIR_SIZE.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_SIZE.SelectionLabel),
            nameof(UserConfig.Current.CrosshairSize));

        NavigationStates.CROSSHAIR_OUTLINE.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean),
            nameof(UserConfig.Current.CrosshairOutline));

        // More
        NavigationStates.TOUCH_MODE.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.TOUCH_MODE.SelectionBoolean),
            nameof(UserConfig.Current.TouchModeEnabled));

        MoreViewModel.Instance.PropertyChanged += OnConfigChanged(
            nameof(MoreViewModel.Instance.HudPosition),
            nameof(UserConfig.Current.HudPosition));

        NavigationStates.STYLE.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.STYLE.SelectionLabel),
            nameof(UserConfig.Current.Style));

        NavigationStates.FONT.PropertyChanged += OnConfigChanged(
            nameof(NavigationStates.FONT.SelectionLabel),
            nameof(UserConfig.Current.Font));
    }

    private static PropertyChangedEventHandler OnConfigChanged(string propertyName, string userConfigPropertyName)
    {
        return (sender, e) =>
        {
            if (propertyName == e.PropertyName)
            {
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    try
                    {
                        //Debug.Print("Save OnConfigChanged {0} {1}", e.PropertyName, sender);

                        Thread.Sleep(100);
                        var src = sender!;
                        var value = src.GetType().GetProperty(propertyName)!.GetValue(src, null);

                        var dest = UserConfig.Current;
                        dest.GetType().GetProperty(userConfigPropertyName)!.SetValue(dest, value);

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        var jsonString = JsonSerializer.Serialize(UserConfig.Current, options);

                        //Debug.Print(jsonString);
                        File.WriteAllText(UserConfigFile, jsonString);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Save Error:");
                        Console.WriteLine(ex.ToString());
                    }
                });
            }
        };
    }

    private static async Task ShowSplash(string text)
    {
        Console.WriteLine($"Setup ShowSplash2 '{text}' {Entry.Millis()}");
        SplashWindowViewModel.Instance.SplashText = text;
        await Task.Delay(TimeSpan.FromMilliseconds(20));
    }

    private static async Task EnforceSingleInstance()
    {
        if (Mutex == null)
        {
            Console.WriteLine("Setup Mutex Ignore");
            return;
        }

        try
        {
            var startRount = 0;
            while (!Mutex.WaitOne(TimeSpan.Zero, true) && startRount <= 10)
            {
                Console.WriteLine("Setup Shutdown existing window (attempt: " + startRount + "/10)");
                WindowsAPI.SendNotifyMessage(new nint(-1), WM_GAME_DIRECT_SHOWME, 0, 0);
                await Task.Delay(500);
                startRount++;
            }
        }
        catch (AbandonedMutexException)
        {
        }

        // never every start second time
        if (!Mutex.WaitOne(TimeSpan.Zero, true))
        {
            await ShowSplash("Could not stop existing window - shutting down.. :(");
            await Task.Delay(1000);
            SplashWindowViewModel.Instance.IsCloseActivated = true;
        }
    }

    private static void SetupLogging()
    {
        if (App.Testing)
            return;

        // configure logging
        var writer = new DebugAndFileWriter(Path.Combine(RootPath, "log.txt"));
        Console.SetOut(writer);
        Console.SetError(writer);
    }

    private static void CreateRootPath()
    {
        if (!Directory.Exists(RootPath))
            Directory.CreateDirectory(RootPath);
    }

    private static void LazyCreateVersion()
    {
        try
        {
            if (!Directory.Exists(VersionPath))
            {
                Console.WriteLine("Setup Creating New Version");
                Directory.CreateDirectory(VersionPath);
                CopyFilesRecursively(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Version"),
                    Path.Combine(VersionPath)
                    );
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Setup Error creating version");
            Console.WriteLine(e);
            Directory.Delete(VersionPath, true);

            throw new Exception("Setup Could not create version", e);
        }
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        // Console.WriteLine("Copy {0}, {1}", sourcePath, targetPath);
        // Now Create all of the directories
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

        // Copy all the files & Replaces any files with the same name
        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }

    private static void ComputePaths()
    {
        try
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            RootPath = Path.Combine(storageFolder.Path, "Game Direct");

            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;
            var Version = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            VersionPath = Path.Combine(RootPath, Version);
            UserConfigFile = Path.Combine(VersionPath, "UserConfig.json");
        }
        catch (Exception)
        {
            RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Direct");
#if HOT
            VersionPath = Path.Combine(RootPath, "0.0.0." + (new Random().NextInt64() >> 48));
#else
            VersionPath = Path.Combine(RootPath, "0.0.0.0");
#endif
            UserConfigFile = Path.Combine(VersionPath, "UserConfig.json");
        }

        Debug.Print($"Setup VersionPath={VersionPath}");
    }

    private static async Task CheckLicense()
    {
        var context = StoreContext.GetDefault();
        var appLicense = await context.GetAppLicenseAsync();

        Console.WriteLine($"Setup License IsActive={appLicense.IsActive} IsTrial={appLicense.IsTrial} ExpirationDate={appLicense.ExpirationDate}");

        if (!appLicense.IsActive)
        {
            SplashWindowViewModel.Instance.IsCloseActivated = true;
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
