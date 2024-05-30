using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Windows.UI;

namespace Hud1.Controls
{

    public class SplashWindowBehavior : Behavior<Window>
    {
#pragma warning disable CS8618
        private Window window;
#pragma warning restore CS8618

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            window = ((Window)sender);

            var hwnd = new WindowInteropHelper(window).Handle;
            var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
            WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
                extendedStyle
                | WindowsAPI.WS_EX_NOACTIVATE
                | WindowsAPI.WS_EX_TRANSPARENT
                );

            Displays.RegisterDisplaysChange(window, OnDisplayChange);
            OnDisplayChange();
        }

        private void OnDisplayChange()
        {
            var PrimaryScreen = Displays.PrimaryScreen;

            //Debug.Print($"SplashWindowBehavior OnDisplayChange {Screen.PrimaryScreen.ScaleFactor} {Screen.PrimaryScreen.WpfBounds} {Screen.PrimaryScreen.Bounds.Width} {window.Width}");
            //var coords = WindowHelper.CalculateWindowCoordinates(window, Helpers.ScreenHelper.Enum.WindowPositions.Center, Screen.PrimaryScreen);

            //Debug.Print($"What {coords.Left} {window.Left}");

            window.Left = PrimaryScreen.Bounds.X + PrimaryScreen.Bounds.Width / 2 - window.Width / 2;
            window.Top = PrimaryScreen.Bounds.Y + PrimaryScreen.Bounds.Height / 2 - window.Height / 2;
        }
    }
}
