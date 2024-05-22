using System.Runtime.InteropServices;

namespace Hud1.Helpers;

public enum GlobalKey : int
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

public static class WindowsAPI
{
    public static readonly nint HWND_TOP = new nint(0);

    public const int WH_KEYBOARD_LL = 13;
    public const int WH_MOUSE_LL = 14;

    public const int WM_KEYDOWN = 0x100;
    public const int WM_KEYUP = 0x101;
    public const int WM_SYSKEYDOWN = 0x0104;
    public const int WM_SYSKEYUP = 0x0105;

    public const int WH_MOUSE = 7;
    public const int WH_KEYBOARD = 2;
    public const int WM_MOUSEMOVE = 0x200;
    public const int WM_LBUTTONDOWN = 0x201;
    public const int WM_RBUTTONDOWN = 0x204;
    public const int WM_MBUTTONDOWN = 0x207;
    public const int WM_LBUTTONUP = 0x202;
    public const int WM_RBUTTONUP = 0x205;
    public const int WM_MBUTTONUP = 0x208;
    public const int WM_LBUTTONDBLCLK = 0x203;
    public const int WM_RBUTTONDBLCLK = 0x206;
    public const int WM_MBUTTONDBLCLK = 0x209;
    public const int WM_MOUSEWHEEL = 0x020A;

    public const int WS_EX_TRANSPARENT = 0x00000020;
    public const int WS_EX_TOOLWINDOW = 0x00000080;
    public const int WS_EX_NOACTIVATE = 0x08000000;
    public const int GWL_EXSTYLE = (-20);

    // Hooks

    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    // Modules

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);


    // Windows

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        SWP_NOACTIVATE = 0x0010,
        SWP_NOMOVE = 0x0002,
        SWP_NOSIZE = 0x0001
    }
    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);


    // Window Style

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    // message

    public const int HWND_BROADCAST = 0xffff;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern uint RegisterWindowMessage(string message);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SendNotifyMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

}