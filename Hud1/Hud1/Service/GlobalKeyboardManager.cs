using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hud1.Service
{
    public enum GlobalKey : int
    {
        VK_LMENU = 0xA4,

        VK_LEFT = 0x25,
        VK_UP = 0x26,
        VK_RIGHT = 0x27,
        VK_DOWN = 0x28,

        VK_F2 = 0x71,

        VK_F = 0x46,
        VK_L = 0x4C,
        VK_S = 0x53
    }

    public class KeyEvent
    {
        public bool alt = false;
        public bool block = false;

        public GlobalKey key;

        public KeyEvent(GlobalKey key)
        {
            this.key = key;
        }
    }

    public static class GlobalKeyboardManager
    {
        // Callbacks
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc LowLevelProc = HookCallback;

        private static Dictionary<GlobalKey, bool> IsDown = new Dictionary<GlobalKey, bool>();

        // The build in proc ID for telling windows to hook onto the
        // low level keyboard events with the SetWindowsHookEx function
        private const int WH_KEYBOARD_LL = 13;

        // The system hook ID (for storing this application's hook)
        private static IntPtr HookID = IntPtr.Zero;
        //        internal static Func<GlobalKey, bool, bool> HandleKeyDown;

        public delegate void KeyDownHandler(KeyEvent keyEvent);
        public static event KeyDownHandler KeyDown;


        /// <summary>
        /// Hooks/Sets up this application for receiving keydown callbacks
        /// </summary>
        public static void SetupSystemHook()
        {
            HookID = SetHook(LowLevelProc);
        }

        /// <summary>
        /// Unhooks this application, stopping it from receiving keydown callbacks
        /// </summary>
        public static void ShutdownSystemHook()
        {
            UnhookWindowsHookEx(HookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
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
            const int WM_KEYDOWN = 0x100;
            const int WM_KEYUP = 0x101;
            const int WM_SYSKEYDOWN = 0x0104;
            const int WM_SYSKEYUP = 0x0105;

            // Debug.Print("nCode {0} {1} {2}", code, wParam, lParam);

            if (code >= 0)
            {
                switch (wParam)
                {
                    case WM_KEYDOWN:
                        {
                            int vkCode = Marshal.ReadInt32(lParam);
                            var keyEvent = new KeyEvent((GlobalKey)vkCode);
                            KeyDown(keyEvent);

                            // Debug.Print("WM_KEYDOWN vkCode:{0} blocked:{1}", vkCode, blocked);
                            if (keyEvent.block)
                            {
                                return 1;
                            }
                            break;
                        }
                    case WM_KEYUP:
                        {
                            int vkCode = Marshal.ReadInt32(lParam);
                            // Debug.Print("WM_KEYUP vkCode:{0}", vkCode);
                            break;
                        }
                    case WM_SYSKEYDOWN:
                        {
                            int vkCode = Marshal.ReadInt32(lParam);
                            // Debug.Print("WM_SYSKEYDOWN vkCode:{0}", vkCode);
                            if (vkCode == (int)GlobalKey.VK_LMENU)
                            {
                                IsDown[GlobalKey.VK_LMENU] = true;
                            }
                            var keyEvent = new KeyEvent((GlobalKey)vkCode);
                            keyEvent.alt = IsDown[GlobalKey.VK_LMENU];
                            KeyDown(keyEvent);
                            // Debug.Print("WM_KEYDOWN vkCode:{0} blocked:{1}", vkCode, blocked);
                            if (keyEvent.block)
                            {
                                return 1;
                            }
                            break;
                        }
                    case WM_SYSKEYUP:
                        {
                            int vkCode = Marshal.ReadInt32(lParam);
                            // Debug.Print("WM_SYSKEYUP vkCode:{0}", vkCode);
                            if (vkCode == (int)GlobalKey.VK_LMENU)
                            {
                                IsDown[GlobalKey.VK_LMENU] = false;
                            }
                            break;
                        }
                    default:
                        {
                            Debug.Print("UNKNOWN wParam:{0}", wParam);
                            break;
                        }
                }
            }

            return CallNextHookEx(HookID, code, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}