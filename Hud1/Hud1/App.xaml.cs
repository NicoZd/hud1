using System.Diagnostics;
using System.Windows;

namespace Hud1;

public partial class App : Application
{
    internal App()
    {
        Debug.Print($"App {ShutdownMode} {Entry.Millis()}");
        InitializeComponent();
    }
}