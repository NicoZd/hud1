using CommunityToolkit.Mvvm.ComponentModel;

namespace Hud1.ViewModels;

internal partial class SplashWindowViewModel : ObservableObject
{
    public static readonly SplashWindowViewModel Instance = new();

    [ObservableProperty]
    private bool _isCloseActivated = false;

    [ObservableProperty]
    private string _splashText = "";

    private SplashWindowViewModel()
    {
    }
}
