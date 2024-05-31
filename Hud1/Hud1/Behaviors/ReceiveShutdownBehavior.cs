using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Behaviors
{
    class ReceiveShutdownBehavior : Behavior<Window>
    {
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
            var window = (Window)AssociatedObject;
            var hwnd = new WindowInteropHelper(window).Handle;
            var source = HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Debug.Print($"SOSO {msg}");
            if (msg == Setup.WM_GAME_DIRECT_SHOWME)
            {
                Console.WriteLine("ReceiveShutdownBehavior WndProc {0} {1}", msg, Setup.WM_GAME_DIRECT_SHOWME);
                Application.Current.Shutdown();
                return IntPtr.Zero;
            }

            return IntPtr.Zero;
        }
    }
}
