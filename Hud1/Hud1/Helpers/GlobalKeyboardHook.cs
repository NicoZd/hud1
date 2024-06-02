using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.System;

namespace Hud1.Helpers;

internal class KeyEvent
{
    internal bool alt = false;
    internal bool block = false;
    internal bool repeated = false;

    internal VirtualKey key;

    internal KeyEvent(VirtualKey key)
    {
        this.key = key;
    }
}

internal static class VirtualKeyboardHook
{
    internal delegate void KeyDownHandler(KeyEvent keyEvent);
    internal static event KeyDownHandler? KeyDown;

    private static IntPtr HookID = IntPtr.Zero;

    private static readonly Dictionary<VirtualKey, bool> IsDown = [];
    private static VirtualKey? _lastPressedKey;

    internal static void SystemHook()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;

        HookID = WindowsAPI.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, HookCallback, WindowsAPI.GetModuleHandle(curModule.ModuleName), 0);
    }

    internal static void SystemUnhook()
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
                case WindowMessage.WM_KEYDOWN:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        var keyEvent = new KeyEvent((VirtualKey)vkCode);
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
                case WindowMessage.WM_KEYUP:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        // Console.WriteLine("WM_KEYUP vkCode:{0}", vkCode);
                        _lastPressedKey = null;
                        break;
                    }
                case WindowMessage.WM_SYSKEYDOWN:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        // Console.WriteLine("WM_SYSKEYDOWN vkCode:{0}", vkCode);
                        if (vkCode == (int)VirtualKey.LeftMenu)
                        {
                            IsDown[VirtualKey.LeftMenu] = true;
                        }
                        var keyEvent = new KeyEvent((VirtualKey)vkCode)
                        {
                            alt = IsDown.ContainsKey(VirtualKey.LeftMenu) && IsDown[VirtualKey.LeftMenu]
                        };
                        KeyDown?.Invoke(keyEvent);
                        // Console.WriteLine("WM_KEYDOWN vkCode:{0} blocked:{1}", vkCode, blocked);
                        if (keyEvent.block)
                        {
                            return 1;
                        }
                        break;
                    }
                case WindowMessage.WM_SYSKEYUP:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        // Console.WriteLine("WM_SYSKEYUP vkCode:{0}", vkCode);
                        if (vkCode == (int)VirtualKey.LeftMenu)
                        {
                            IsDown[VirtualKey.LeftMenu] = false;
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