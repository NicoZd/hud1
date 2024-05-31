using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public static readonly MainWindowViewModel Instance = new();

    [ObservableProperty]
    public bool _active = true;

    [ObservableProperty]
    public bool _isForeground = false;

    [ObservableProperty]
    public Visibility _hudVisibility = Visibility.Visible;

    private MainWindowViewModel()
    {
    }

    internal void HandleKeyActivator()
    {
        Console.WriteLine("HandleKeyActivator");

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
