using System.Diagnostics;

namespace Hud1.Helpers;

internal static class GlobalMouseHook
{
    internal delegate void MouseDownHandler(int button);
    internal static event MouseDownHandler? MouseDown;

    internal delegate void MouseUpHandler(int button);
    internal static event MouseUpHandler? MouseUp;

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
            try
            {

                switch (wParam)
                {
                    case WindowMessage.WM_LBUTTONDOWN:
                        {
                            MouseDown?.Invoke(1);
                            break;
                        }
                    case WindowMessage.WM_RBUTTONDOWN:
                        {
                            MouseDown?.Invoke(2);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONDOWN:
                        {
                            MouseDown?.Invoke(3);
                            break;
                        }

                    case WindowMessage.WM_LBUTTONUP:
                        {
                            MouseUp?.Invoke(1);
                            break;
                        }
                    case WindowMessage.WM_RBUTTONUP:
                        {
                            MouseUp?.Invoke(2);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONUP:
                        {
                            MouseUp?.Invoke(3);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling mouse events.");
                Console.WriteLine(ex.ToString());
            }

        }

        return WindowsAPI.CallNextHookEx(HookID, code, wParam, lParam);
    }
}