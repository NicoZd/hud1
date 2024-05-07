using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Hud1.Helpers
{
    public static class GlobalMouseHook
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public delegate void MouseDownHandler();
        public static event MouseDownHandler? MouseDown;

        public delegate void MouseMoveHandler(int x, int y);
        public static event MouseMoveHandler? MouseMove;

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
                    case WindowsAPI.WM_MOUSEMOVE:
                        {
                            MSLLHOOKSTRUCT mouseStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                            int x2 = mouseStruct.pt.X;
                            int y2 = mouseStruct.pt.Y;

                            Debug.Print("WM_MOUSEMOVE {0} {1} {2} {3}", x2, y2, mouseStruct.dwExtraInfo, mouseStruct.mouseData);

                            MouseMove?.Invoke(x2, y2);

                            mouseStruct.pt.X = 0;

                            //return 1;
                            return WindowsAPI.CallNextHookEx(HookID, -1, 0, lParam);

                            //return 1;
                            //return new IntPtr(-1);
                            break;
                        }
                    default:
                        {
                            Debug.Print("??? {0} ", wParam);
                            break;
                        }
                }
            }

            return WindowsAPI.CallNextHookEx(HookID, code, wParam, lParam);
        }
    }
}