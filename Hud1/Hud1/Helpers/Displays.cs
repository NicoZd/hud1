using Hud1.Helpers.ScreenHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Helpers
{
    class MonitorEnumCallback
    {
        public MonitorEnumCallback()
        {
            Screens = [];
        }

        public ArrayList Screens { get; }

        public bool Callback(nint monitor, nint hdc, nint lprcMonitor, nint lparam)
        {
            Debug.Print("CallBACK", nameof(Screen));
            Screens.Add(new Screen2(monitor, hdc));
            return true;
        }
    }

    class Screen2
    {
        private const int MONITORINFOF_PRIMARY = 0x00000001;

        public readonly double ScaleFactor;
        public readonly Rect Bounds;
        public readonly bool IsPrimary;

        public Screen2()
        {
            Debug.Print("DEFAULT MONITOR");
            ScaleFactor = 1.0;
            Bounds = new Rect(0, 0, 1920, 1080);
            IsPrimary = true;
        }

        public Screen2(nint monitor, nint hdc)
        {
            NativeMethods.GetDpiForMonitor(monitor, NativeMethods.DpiType.EFFECTIVE, out var dpiX, out _);
            ScaleFactor = dpiX / 96.0;

            var info = new NativeMethods.MONITORINFOEX();
            NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), info);
            Bounds = new Rect(
                info.rcMonitor.left,
                info.rcMonitor.top,
                info.rcMonitor.right - info.rcMonitor.left,
                info.rcMonitor.bottom - info.rcMonitor.top);
            Bounds.Scale(1 / ScaleFactor, 1 / ScaleFactor);
            IsPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;
        }
    }

    class Displays
    {

        public static IEnumerable<Screen2> All
        {
            get
            {
                var closure = new MonitorEnumCallback();
                var proc = new NativeMethods.MonitorEnumProc(closure.Callback);

                NativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, proc, nint.Zero);
                return closure.Screens.Cast<Screen2>();
            }
        }

        public static Screen2 PrimaryScreen
        {
            get
            {
                return All.FirstOrDefault(t => t.IsPrimary) ?? new Screen2();
            }
        }


        public static void RegisterDisplaysChange(Window window, Action OnDisplayChange)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var source = HwndSource.FromHwnd(hwnd);

            IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (msg == 126)
                {
                    Debug.Print("DisplaysChange...");
                    OnDisplayChange();
                    return IntPtr.Zero;
                }
                return IntPtr.Zero;
            }

            source.AddHook(WndProc);
        }
    }
}
