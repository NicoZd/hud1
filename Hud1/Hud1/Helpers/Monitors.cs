using Hud1.Helpers.ScreenHelper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Helpers;

internal class MonitorEnumCallback
{
    public readonly List<Monitor> Monitors = [];

    public bool Callback(nint monitor, nint hdc, nint lprcMonitor, nint lparam)
    {
        Monitors.Add(new Monitor(monitor, hdc));
        return true;
    }
}

internal class Monitor
{
    private const int MONITORINFOF_PRIMARY = 0x00000001;

    public readonly double ScaleFactor;
    public readonly Rect Bounds;
    public readonly bool IsPrimary;

    public Monitor()
    {
        Debug.Print("DEFAULT MONITOR");
        ScaleFactor = 1.0;
        Bounds = new Rect(0, 0, 1920, 1080);
        IsPrimary = true;
    }

    public Monitor(nint monitor, nint hdc)
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

internal class Monitors
{

    public static List<Monitor> All
    {
        get
        {
            var closure = new MonitorEnumCallback();
            var proc = new NativeMethods.MonitorEnumProc(closure.Callback);
            NativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, proc, nint.Zero);
            return closure.Monitors;
        }
    }

    public static Monitor Primary => All.FirstOrDefault(t => t.IsPrimary) ?? new Monitor();


    public static void RegisterMonitorsChange(Window window, Action OnMonitorsChange)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var source = HwndSource.FromHwnd(hwnd);

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 126)
            {
                Debug.Print("OnMonitorsChange...");
                OnMonitorsChange();
                return IntPtr.Zero;
            }
            return IntPtr.Zero;
        }

        source.AddHook(WndProc);
    }
}
