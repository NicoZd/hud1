using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Helpers.ScreenHelper.Enum;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Hud1.Windows;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        Debug.Print("SplashWindow {0}", Entry.Millis());
        Application.Current.MainWindow = this;
        InitializeComponent();
    }
}
