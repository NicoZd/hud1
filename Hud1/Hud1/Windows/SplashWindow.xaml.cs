using System.Diagnostics;
using System.Windows;

namespace Hud1.Windows;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        Debug.Print("SplashWindow {0}", Entry.Millis());
        Application.Current.MainWindow = this;
        Opacity = 0;
        InitializeComponent();
    }
}
