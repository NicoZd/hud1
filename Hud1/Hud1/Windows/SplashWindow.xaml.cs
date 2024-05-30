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

        //DataContext = mew SplashWindowViewModel.Instance;

        //Opacity = 0;


        //((SplashWindowViewModel)DataContext).OnWindowLoaded(this);
        //SplashWindowViewModel.Instance.OnWindowLoaded(this);
        InitializeComponent();

    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {


        //this.SetWindowPosition(WindowPositions.Center, Screen.PrimaryScreen);



        //FadeIn();
    }

    private void FadeIn()
    {
        var animation = new DoubleAnimation
        {
            To = 1,
            BeginTime = TimeSpan.FromSeconds(0),
            Duration = TimeSpan.FromSeconds(0.15),
            FillBehavior = FillBehavior.Stop
        };
        animation.Completed += async (s, a) =>
        {
            Debug.Print("SplashWindow Animation In Complete {0}", Entry.Millis());
            Opacity = 1;
            await StartupAndShowMainWindow();

        };
        BeginAnimation(UIElement.OpacityProperty, animation);
    }

    private async Task StartupAndShowMainWindow()
    {
        try
        {
            await Setup.Run();
            MainWindow.Create();
            CloseWithAnimation();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Opacity = 0;
            MessageBox.Show("Wooo - there was a fatal startup error:\n\n" + ex.ToString(), "Game Direct", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void CloseWithAnimation()
    {
        var animation = new DoubleAnimation
        {
            To = 0,
            BeginTime = TimeSpan.FromSeconds(0),
            Duration = TimeSpan.FromSeconds(0.15),
            FillBehavior = FillBehavior.Stop
        };
        animation.Completed += (s, a) =>
        {
            Debug.Print("SplashWindow Animation Out Complete {0}", Entry.Millis());
            Opacity = 0;
            Close();
        };
        BeginAnimation(UIElement.OpacityProperty, animation);
    }
}
