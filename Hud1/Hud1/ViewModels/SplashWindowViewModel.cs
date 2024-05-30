using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.ViewModels
{
    partial class Position : ObservableObject
    {
        [ObservableProperty]
        private double _left;

        [ObservableProperty]
        private double _top;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;
    }

    partial class SplashWindowViewModel : ObservableObject
    {
        public static readonly SplashWindowViewModel Instance = new();

        [ObservableProperty]
        private Position _position = new Position() { Left = 0, Top = 0 };

        [ObservableProperty]
        private bool _isCloseActivated = false;

        [ObservableProperty]
        private string _splashText = "hello";

        private SplashWindowViewModel()
        {
        }

        [RelayCommand]
        private void Load(Window window)
        {
            Debug.Print($"Load {window.Width}");
            //var coords = WindowHelper.CalculateWindowCoordinates(window, Helpers.ScreenHelper.Enum.WindowPositions.Center, Screen.PrimaryScreen);
            //Debug.Print($"{coords.Left} {coords.Top}");
            //Position.Left = coords.Left;
            //Position.Top = coords.Top;

            //Debug.Print("SplashWindow OnWindowLoaded {0}", Entry.Millis());
            //var hwnd = new WindowInteropHelper(window).Handle;
            //var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
            //WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
            //    extendedStyle
            //    | WindowsAPI.WS_EX_NOACTIVATE
            //    | WindowsAPI.WS_EX_TRANSPARENT
            //    );
        }
    }
}
