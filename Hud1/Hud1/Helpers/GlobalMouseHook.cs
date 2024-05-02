using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hud1.Helpers
{

    //GlobalKeyboardHook
    //GlobalMouseHook
    public static class GlobalMouseHook
    {
        const int WH_MOUSE = 7;
        const int WH_KEYBOARD = 2;
        const int WM_MOUSEMOVE = 0x200;
        const int WM_LBUTTONDOWN = 0x201;
        const int WM_RBUTTONDOWN = 0x204;
        const int WM_MBUTTONDOWN = 0x207;
        const int WM_LBUTTONUP = 0x202;
        const int WM_RBUTTONUP = 0x205;
        const int WM_MBUTTONUP = 0x208;
        const int WM_LBUTTONDBLCLK = 0x203;
        const int WM_RBUTTONDBLCLK = 0x206;
        const int WM_MBUTTONDBLCLK = 0x209;
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        // Callbacks
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc LowLevelProc = HookCallback;

        // The build in proc ID for telling windows to hook onto the
        // low level keyboard events with the SetWindowsHookEx function
        private const int WH_MOUSE_LL = 14;

        // The system hook ID (for storing this application's hook)
        private static IntPtr HookID = IntPtr.Zero;

        public delegate void MouseDownHandler();
        public static event MouseDownHandler? MouseDown;


        /// <summary>
        /// Hooks/Sets up this application for receiving keydown callbacks
        /// </summary>
        public static void SystemHook()
        {
            HookID = SetHook(LowLevelProc);
        }

        /// <summary>
        /// Unhooks this application, stopping it from receiving keydown callbacks
        /// </summary>
        public static void SystemUnhook()
        {
            UnhookWindowsHookEx(HookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule!)
                {
                    return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        /// <summary>
        /// The method called when a key up/down occours across the system.
        /// </summary>
        /// <param name="nCode">idk tbh</param>
        /// <param name="lParam">LPARAM, contains the key that was pressed. not used atm</param>
        /// <returns>LRESULT</returns>
        private static IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code > -1 && !MouseService.IgnoreNextEvent)
            {
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
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

            return CallNextHookEx(HookID, code, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}