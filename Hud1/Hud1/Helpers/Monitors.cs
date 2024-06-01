using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Helpers;

internal class MonitorEnumCallback
{
    internal readonly List<Monitor> Monitors = [];

    internal bool Callback(nint monitor, nint hdc, nint lprcMonitor, nint lparam)
    {
        Monitors.Add(new Monitor(monitor, hdc));
        return true;
    }
}

internal class Monitor
{
    private const int MONITORINFOF_PRIMARY = 0x00000001;

    internal readonly double ScaleFactor;
    internal readonly Rect Bounds;
    internal readonly bool IsPrimary;
    internal readonly string DeviceName;

    internal Monitor()
    {
        Debug.Print("DEFAULT MONITOR");
        ScaleFactor = 1.0;
        Bounds = new Rect(0, 0, 1920, 1080);
        IsPrimary = true;
        DeviceName = "DISPLAY";
    }

    internal Monitor(nint monitor, nint hdc)
    {
        WindowsAPI.GetDpiForMonitor(monitor, WindowsAPI.DpiType.EFFECTIVE, out var dpiX, out _);
        ScaleFactor = dpiX / 96.0;

        var info = new WindowsAPI.MONITORINFOEX();
        WindowsAPI.GetMonitorInfo(new HandleRef(null, monitor), info);
        Bounds = new Rect(
            info.rcMonitor.left,
            info.rcMonitor.top,
            info.rcMonitor.right - info.rcMonitor.left,
            info.rcMonitor.bottom - info.rcMonitor.top);
        IsPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;
        DeviceName = new string(info.szDevice).TrimEnd((char)0);
    }
}

internal class Monitors
{
    internal static List<Monitor> All
    {
        get
        {
            var closure = new MonitorEnumCallback();
            var proc = new WindowsAPI.MonitorEnumProc(closure.Callback);
            WindowsAPI.EnumDisplayMonitors(WindowsAPI.NullHandleRef, null, proc, nint.Zero);
            return closure.Monitors;
        }
    }

    internal static Monitor Primary => All.FirstOrDefault(t => t.IsPrimary) ?? new Monitor();

    internal static Action RegisterMonitorsChange(Window window, Action OnMonitorsChange)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var source = HwndSource.FromHwnd(hwnd);

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var WM_DISPLAYCHANGE = 126;
            if (msg == WM_DISPLAYCHANGE)
            {
                Debug.Print("OnMonitorsChange...");
                OnMonitorsChange();
                return IntPtr.Zero;
            }
            return IntPtr.Zero;
        }

        source.AddHook(WndProc);

        return () => { source.RemoveHook(WndProc); };
    }

    internal static void MoveWindow(Window window, double x, double y, double width, double height)
    {
        // Debug.Print($"MoveWindow {x}, {y}, {width}, {height}");
        var hwnd = new WindowInteropHelper(window).Handle;
        WindowsAPI.MoveWindow(hwnd, (int)(x + 1), (int)(y + 1), (int)(width - 2), (int)(height - 2), false);
        WindowsAPI.MoveWindow(hwnd, (int)x, (int)y, (int)width, (int)height, true);
    }
}
