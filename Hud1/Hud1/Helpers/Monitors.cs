﻿using System.Diagnostics;
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
    public readonly string DeviceName;

    public Monitor()
    {
        Debug.Print("DEFAULT MONITOR");
        ScaleFactor = 1.0;
        Bounds = new Rect(0, 0, 1920, 1080);
        IsPrimary = true;
        DeviceName = "DISPLAY";
    }

    public Monitor(nint monitor, nint hdc)
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
    public static List<Monitor> All
    {
        get
        {
            var closure = new MonitorEnumCallback();
            var proc = new WindowsAPI.MonitorEnumProc(closure.Callback);
            WindowsAPI.EnumDisplayMonitors(WindowsAPI.NullHandleRef, null, proc, nint.Zero);
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
    }

    internal static void MoveWindow(nint hwnd, double x, double y, double width, double height)
    {
        Debug.Print($"MoveWindow {x}, {y}, {width}, {height}");
        WindowsAPI.MoveWindow(hwnd, (int)(x + 1), (int)(y + 1), (int)(width - 2), (int)(height - 2), false);
        WindowsAPI.MoveWindow(hwnd, (int)x, (int)y, (int)width, (int)height, true);
    }
}
