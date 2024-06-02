using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Hud1.Helpers;

internal static class WindowMessage
{
    internal const int WM_KEYDOWN = 0x100;
    internal const int WM_KEYUP = 0x101;
    internal const int WM_SYSKEYDOWN = 0x0104;
    internal const int WM_SYSKEYUP = 0x0105;
    internal const int WM_MOUSEMOVE = 0x200;
    internal const int WM_LBUTTONDOWN = 0x201;
    internal const int WM_RBUTTONDOWN = 0x204;
    internal const int WM_MBUTTONDOWN = 0x207;
    internal const int WM_LBUTTONUP = 0x202;
    internal const int WM_RBUTTONUP = 0x205;
    internal const int WM_MBUTTONUP = 0x208;
    internal const int WM_LBUTTONDBLCLK = 0x203;
    internal const int WM_RBUTTONDBLCLK = 0x206;
    internal const int WM_MBUTTONDBLCLK = 0x209;
    internal const int WM_MOUSEWHEEL = 0x020A;
}

internal static class HookType
{
    internal const int WH_KEYBOARD_LL = 13;
    internal const int WH_MOUSE_LL = 14;
    internal const int WH_MOUSE = 7;
    internal const int WH_KEYBOARD = 2;
}

internal static class WindowConstants
{
    internal const int HWND_TOP = 0;
    internal const int HWND_BROADCAST = 0xffff;

    internal const int WS_EX_TRANSPARENT = 0x00000020;
    internal const int WS_EX_TOOLWINDOW = 0x00000080;
    internal const int WS_EX_NOACTIVATE = 0x08000000;
    internal const int GWL_EXSTYLE = -20;

    internal const int EVENT_SYSTEM_FOREGROUND = 3;
    internal const int WINEVENT_OUTOFCONTEXT = 0;

    internal const int GW_HWNDNEXT = 2;

    internal const int CURSOR_SHOWING = 0x00000001;
}

[Flags]
internal enum SetWindowPosFlags : int
{
    SWP_NOACTIVATE = 0x0010,
    SWP_NOMOVE = 0x0002,
    SWP_NOSIZE = 0x0001,
    SWP_NOZORDER = 0x0004
}

[StructLayout(LayoutKind.Sequential)]
internal struct CURSORINFO
{
    internal int cbSize;
    internal int flags;
    internal IntPtr hCursor;
    internal POINTAPI ptScreenPos;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINTAPI
{
    internal int x;
    internal int y;
}

internal enum DpiType
{
    EFFECTIVE = 0,
    ANGULAR = 1,
    RAW = 2
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    internal int left;
    internal int top;
    internal int right;
    internal int bottom;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
internal class MONITORINFOEX
{
    internal int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));

    internal RECT rcMonitor = new();
    internal RECT rcWork = new();
    internal int dwFlags = 0;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    internal char[] szDevice = new char[32];
}

[StructLayout(LayoutKind.Sequential)]
internal class COMRECT
{
    internal int bottom;
    internal int left;
    internal int right;
    internal int top;
}

internal static class WindowsAPI
{
    // Hooks

    internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    // Modules

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    // Windows

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

    [DllImport("user32.dll")]
    internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("user32.dll")]
    internal static extern nint GetDesktopWindow();

    [DllImport("user32.dll")]
    internal static extern nint GetTopWindow(nint hwnd);

    [DllImport("user32.dll")]
    internal static extern nint GetWindow(nint hwnd, uint uCmd);

    [DllImport("user32.dll")]
    internal static extern bool IsWindowVisible(nint hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    internal static string GetWindowTitle(IntPtr hWnd)
    {
        var length = GetWindowTextLength(hWnd) + 1;
        var title = new StringBuilder(length);
        GetWindowText(hWnd, title, length);
        return title.ToString();
    }

    // Window Style

    [DllImport("user32.dll")]
    internal static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    // message

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern uint RegisterWindowMessage(string message);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SendNotifyMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

    // cursor

    [DllImport("user32.dll")]
    internal static extern bool GetCursorInfo(out CURSORINFO pci);

    internal static bool IsMouseHidden()
    {
        CURSORINFO pci;
        pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
        if (GetCursorInfo(out pci))
        {
            if (pci.flags == 0)
            {
                return true;
            }
        }
        return false;
    }

    // mouse

    [DllImport("user32.dll")]
    internal static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

    // Monitors

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("shcore.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern nint GetDpiForMonitor([In] nint hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern bool GetMonitorInfo(HandleRef hmonitor, [In][Out] MONITORINFOEX info);

    internal delegate bool MonitorEnumProc(nint monitor, nint hdc, nint lprcMonitor, nint lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern bool EnumDisplayMonitors(HandleRef hdc, COMRECT? rcClip, MonitorEnumProc lpfnEnum, nint dwData);

    internal static readonly HandleRef NullHandleRef = new(null, nint.Zero);

    [DllImport("gdi32.dll")]
    internal static extern unsafe bool SetDeviceGammaRamp(int hdc, void* ramp);

    [DllImport("gdi32.dll")]
    internal static extern IntPtr CreateDC(string lpszDriver, string? lpszDevice, string? lpszOutput, IntPtr lpInitData);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteDC(IntPtr hdc);
}