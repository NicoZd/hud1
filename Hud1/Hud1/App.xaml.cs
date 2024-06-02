using Hud1.Helpers;
using Hud1.Views;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Windows.Networking;

namespace Hud1;

public partial class App : Application
{
    internal App()
    {
        Debug.Print($"App {ShutdownMode} {Entry.Millis()}");
        InitializeComponent();
    }
}