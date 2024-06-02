using System.Diagnostics;

namespace Hud1.Helpers;

internal static class GlobalMouseHook
{
    internal delegate void MouseDownHandler();
    internal static event MouseDownHandler? MouseDown;

    private static IntPtr HookID = IntPtr.Zero;

    internal static void SystemHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        HookID = WindowsAPI.SetWindowsHookEx(HookType.WH_MOUSE_LL, HookCallback, WindowsAPI.GetModuleHandle(curModule.ModuleName), 0);
    }

    internal static void SystemUnhook()
    {
        WindowsAPI.UnhookWindowsHookEx(HookID);
    }

    private static IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code > -1 && !MouseService.IgnoreNextEvent)
        {
            switch (wParam)
            {
                case WindowMessage.WM_LBUTTONDOWN:
                    {
                        MouseDown?.Invoke();
                        break;
                    }
            }
        }

        return WindowsAPI.CallNextHookEx(HookID, code, wParam, lParam);
    }
}