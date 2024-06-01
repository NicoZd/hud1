using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Windows;

namespace Hud1.Helpers;

internal enum GlobalKey : int
{
    VK_LMENU = 0xA4,

    VK_LEFT = 0x25,
    VK_UP = 0x26,
    VK_RIGHT = 0x27,
    VK_DOWN = 0x28,

    VK_F2 = 0x71,
    VK_F3 = 0x72,

    VK_F = 0x46,
    VK_L = 0x4C,
    VK_S = 0x53
}

internal static class WindowsAPI
{
    internal static readonly nint HWND_TOP = new(0);
    internal static uint GW_HWNDNEXT = 2;

    internal const int WH_KEYBOARD_LL = 13;
    internal const int WH_MOUSE_LL = 14;

    internal const int WM_KEYDOWN = 0x100;
    internal const int WM_KEYUP = 0x101;
    internal const int WM_SYSKEYDOWN = 0x0104;
    internal const int WM_SYSKEYUP = 0x0105;

    internal const int WH_MOUSE = 7;
    internal const int WH_KEYBOARD = 2;
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

    internal const int WS_EX_TRANSPARENT = 0x00000020;
    internal const int WS_EX_TOOLWINDOW = 0x00000080;
    internal const int WS_EX_NOACTIVATE = 0x08000000;
    internal const int GWL_EXSTYLE = -20;

    // Hooks

    internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    internal const uint EVENT_SYSTEM_FOREGROUND = 3;
    internal const int WINEVENT_OUTOFCONTEXT = 0;

    internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    // Modules

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    // Windows

    [Flags]
    internal enum SetWindowPosFlags : uint
    {
        SWP_NOACTIVATE = 0x0010,
        SWP_NOMOVE = 0x0002,
        SWP_NOSIZE = 0x0001,
        SWP_NOZORDER = 0x0004
    }
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

    internal const int HWND_BROADCAST = 0xffff;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern uint RegisterWindowMessage(string message);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SendNotifyMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

    // cursor

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

    internal const int CURSOR_SHOWING = 0x00000001;
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

    // Monitors

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    internal enum DpiType
    {
        EFFECTIVE = 0,
        ANGULAR = 1,
        RAW = 2
    }

    [DllImport("shcore.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern nint GetDpiForMonitor([In] nint hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern bool GetMonitorInfo(HandleRef hmonitor, [In][Out] MONITORINFOEX info);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        internal int left;
        internal int top;
        internal int right;
        internal int bottom;

        internal RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        internal RECT(Rect r)
        {
            left = (int)r.Left;
            top = (int)r.Top;
            right = (int)r.Right;
            bottom = (int)r.Bottom;
        }

        internal static RECT FromXYWH(int x, int y, int width, int height)
        {
            return new RECT(x, y, x + width, y + height);
        }

        internal Size Size => new(right - left, bottom - top);
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

    internal delegate bool MonitorEnumProc(nint monitor, nint hdc, nint lprcMonitor, nint lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern bool EnumDisplayMonitors(HandleRef hdc, COMRECT? rcClip, MonitorEnumProc lpfnEnum, nint dwData);

    internal static readonly HandleRef NullHandleRef = new(null, nint.Zero);

    [StructLayout(LayoutKind.Sequential)]
    internal class COMRECT
    {
        internal int bottom;
        internal int left;
        internal int right;
        internal int top;

        internal COMRECT()
        {
        }

        internal COMRECT(Rect r)
        {
            left = (int)r.X;
            top = (int)r.Y;
            right = (int)r.Right;
            bottom = (int)r.Bottom;
        }

        internal COMRECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        internal static COMRECT FromXYWH(int x, int y, int width, int height)
        {
            return new COMRECT(x, y, x + width, y + height);
        }

        public override string ToString()
        {
            return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
        }
    }


}