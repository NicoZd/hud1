using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;

namespace Hud1.ViewModels;

internal partial class SplashWindowViewModel : ObservableObject
{
    public static readonly SplashWindowViewModel Instance = new();

    [ObservableProperty]
    private bool _isCloseActivated = false;

    [ObservableProperty]
    private string _splashText = "hello";

    [ObservableProperty]
    private bool _fadeIn = false;

    private SplashWindowViewModel()
    {
    }

    [RelayCommand]
    private async Task Load()
    {
        Debug.Print($"Load");

        FadeIn = true;
        await Task.Delay(150);

        try
        {
            await Setup.Run();
            MainWindow.Create();
            FadeIn = false;
            await Task.Delay(150);
            IsCloseActivated = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            FadeIn = false;
            MessageBox.Show("Wooo - there was a fatal startup error:\n\n" + ex.ToString(), "Game Direct", MessageBoxButton.OK, MessageBoxImage.Error);
            IsCloseActivated = true;
        }

    }
}
