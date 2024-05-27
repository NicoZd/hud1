using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hud1.Helpers;

public class KeyEvent
{
    public bool alt = false;
    public bool block = false;
    public bool repeated = false;

    public GlobalKey key;

    public KeyEvent(GlobalKey key)
    {
        this.key = key;
    }
}

public static class GlobalKeyboardHook
{
    public delegate void KeyDownHandler(KeyEvent keyEvent);
    public static event KeyDownHandler? KeyDown;

    private static IntPtr HookID = IntPtr.Zero;

    private static readonly Dictionary<GlobalKey, bool> IsDown = [];
    private static GlobalKey? _lastPressedKey;

    public static void SystemHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        HookID = WindowsAPI.SetWindowsHookEx(WindowsAPI.WH_KEYBOARD_LL, HookCallback, WindowsAPI.GetModuleHandle(curModule.ModuleName), 0);
    }

    public static void SystemUnhook()
    {
        WindowsAPI.UnhookWindowsHookEx(HookID);
    }

    private static IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
    {


        // Console.WriteLine("nCode {0} {1} {2}", code, wParam, lParam);

        if (code >= 0)
        {
            switch (wParam)
            {
                case WindowsAPI.WM_KEYDOWN:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        var keyEvent = new KeyEvent((GlobalKey)vkCode);
                        keyEvent.repeated = keyEvent.key.Equals(_lastPressedKey);
                        _lastPressedKey = keyEvent.key;
                        KeyDown?.Invoke(keyEvent);
                        // Console.WriteLine("WM_KEYDOWN vkCode:{0} blocked:{1}", vkCode, blocked);
                        if (keyEvent.block)
                        {
                            return 1;
                        }
                        break;
                    }
                case WindowsAPI.WM_KEYUP:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        // Console.WriteLine("WM_KEYUP vkCode:{0}", vkCode);
                        _lastPressedKey = null;
                        break;
                    }
                case WindowsAPI.WM_SYSKEYDOWN:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        // Console.WriteLine("WM_SYSKEYDOWN vkCode:{0}", vkCode);
                        if (vkCode == (int)GlobalKey.VK_LMENU)
                        {
                            IsDown[GlobalKey.VK_LMENU] = true;
                        }
                        var keyEvent = new KeyEvent((GlobalKey)vkCode)
                        {
                            alt = IsDown.ContainsKey(GlobalKey.VK_LMENU) && IsDown[GlobalKey.VK_LMENU]
                        };
                        KeyDown?.Invoke(keyEvent);
                        // Console.WriteLine("WM_KEYDOWN vkCode:{0} blocked:{1}", vkCode, blocked);
                        if (keyEvent.block)
                        {
                            return 1;
                        }
                        break;
                    }
                case WindowsAPI.WM_SYSKEYUP:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        // Console.WriteLine("WM_SYSKEYUP vkCode:{0}", vkCode);
                        if (vkCode == (int)GlobalKey.VK_LMENU)
                        {
                            IsDown[GlobalKey.VK_LMENU] = false;
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("UNKNOWN wParam:{0}", wParam);
                        break;
                    }
            }
        }

        return WindowsAPI.CallNextHookEx(HookID, code, wParam, lParam);
    }
}