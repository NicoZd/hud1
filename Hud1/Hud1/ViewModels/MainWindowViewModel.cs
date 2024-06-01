using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Hud1.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    public static readonly MainWindowViewModel Instance = new();

    [ObservableProperty]
    private bool _active = true;

    [ObservableProperty]
    private bool _isForeground = false;

    [ObservableProperty]
    private Visibility _hudVisibility = Visibility.Visible;

    private MainWindowViewModel()
    {
    }

    internal void Activate()
    {
        Active = true;
        HudVisibility = Visibility.Visible;
    }

    internal void ToggleActive()
    {
        if (Active)
        {
            HudVisibility = Visibility.Collapsed;
            Active = false;
        }
        else
        {
            HudVisibility = Visibility.Visible;
            Active = true;
        }
    }

}
