using System.Diagnostics;
using System.Windows;

namespace Hud1;

public partial class App : Application
{
    public static bool Testing { get; set; } = false;

    public App()
    {
        Debug.Print($"App {ShutdownMode} {Entry.Millis()}");
        InitializeComponent();
    }
}