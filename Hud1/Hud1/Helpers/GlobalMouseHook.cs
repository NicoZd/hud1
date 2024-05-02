using System.Diagnostics;

namespace Hud1.Helpers
{
    public static class GlobalMouseHook
    {
        public delegate void MouseDownHandler();
        public static event MouseDownHandler? MouseDown;

        private static IntPtr HookID = IntPtr.Zero;

        public static void SystemHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule!)
                {
                    HookID = WindowsAPI.SetWindowsHookEx(WindowsAPI.WH_MOUSE_LL, HookCallback, WindowsAPI.GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        public static void SystemUnhook()
        {
            WindowsAPI.UnhookWindowsHookEx(HookID);
        }

        private static IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code > -1 && !MouseService.IgnoreNextEvent)
            {
                switch (wParam)
                {
                    case WindowsAPI.WM_LBUTTONDOWN:
                        {
                            MouseDown?.Invoke();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return WindowsAPI.CallNextHookEx(HookID, code, wParam, lParam);
        }
    }
}