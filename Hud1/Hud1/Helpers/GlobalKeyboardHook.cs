using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Windows.System;

namespace Hud1.Helpers;

public class KeyEvent
{
    public bool alt = false;
    public bool shift = false;
    public bool block = false;
    public bool repeated = false;

    public VirtualKey key;

    public KeyEvent(VirtualKey key)
    {
        this.key = key;
    }
}

internal static class GlobalKeyboardHook
{
    internal delegate void KeyDownHandler(KeyEvent keyEvent);
    internal static event KeyDownHandler? KeyDown;

    internal delegate void KeyUpHandler(KeyEvent keyEvent);
    internal static event KeyUpHandler? KeyUp;

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
        if (code >= 0)
        {
            switch (wParam)
            {
                case WindowMessage.WM_KEYDOWN:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);

                        Console.WriteLine($"WM_KEYDOWN {vkCode}");

                        if (vkCode == (int)VirtualKey.LeftShift) IsDown[VirtualKey.LeftShift] = true;
                        if (vkCode == (int)VirtualKey.RightShift) IsDown[VirtualKey.RightShift] = true;
                        if (vkCode == (int)VirtualKey.LeftMenu) IsDown[VirtualKey.LeftMenu] = true;
                        if (vkCode == (int)VirtualKey.RightMenu) IsDown[VirtualKey.RightMenu] = true;

                        var keyEvent = new KeyEvent((VirtualKey)vkCode)
                        {
                            alt = CheckIsDown(VirtualKey.LeftMenu) || CheckIsDown(VirtualKey.RightMenu),
                            shift = CheckIsDown(VirtualKey.LeftShift) || CheckIsDown(VirtualKey.RightShift)
                        };

                        keyEvent.repeated = keyEvent.key.Equals(_lastPressedKey);
                        _lastPressedKey = keyEvent.key;
                        KeyDown?.Invoke(keyEvent);
                        if (keyEvent.block)
                        {
                            return 1;
                        }
                        break;
                    }
                case WindowMessage.WM_KEYUP:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        Console.WriteLine($"WM_KEYUP {vkCode}");

                        if (vkCode == (int)VirtualKey.LeftMenu) IsDown[VirtualKey.LeftMenu] = false;
                        if (vkCode == (int)VirtualKey.RightMenu) IsDown[VirtualKey.RightMenu] = false;
                        if (vkCode == (int)VirtualKey.LeftShift) IsDown[VirtualKey.LeftShift] = false;
                        if (vkCode == (int)VirtualKey.RightShift) IsDown[VirtualKey.RightShift] = false;

                        var keyEvent = new KeyEvent((VirtualKey)vkCode)
                        {
                            alt = CheckIsDown(VirtualKey.LeftMenu) || CheckIsDown(VirtualKey.RightMenu),
                            shift = CheckIsDown(VirtualKey.LeftShift) || CheckIsDown(VirtualKey.RightShift)
                        };
                        keyEvent.repeated = keyEvent.key.Equals(_lastPressedKey);

                        _lastPressedKey = null;
                        KeyUp?.Invoke(keyEvent);
                        break;
                    }
                case WindowMessage.WM_SYSKEYDOWN:
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        Console.WriteLine($"WM_SYSKEYDOWN {vkCode}");
                        if (vkCode == (int)VirtualKey.LeftMenu) IsDown[VirtualKey.LeftMenu] = true;
                        if (vkCode == (int)VirtualKey.RightMenu) IsDown[VirtualKey.RightMenu] = true;
                        if (vkCode == (int)VirtualKey.LeftShift) IsDown[VirtualKey.LeftShift] = true;
                        if (vkCode == (int)VirtualKey.RightShift) IsDown[VirtualKey.RightShift] = true;

                        var keyEvent = new KeyEvent((VirtualKey)vkCode)
                        {
                            alt = CheckIsDown(VirtualKey.LeftMenu) || CheckIsDown(VirtualKey.RightMenu),
                            shift = CheckIsDown(VirtualKey.LeftShift) || CheckIsDown(VirtualKey.RightShift)
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

                        if (vkCode == (int)VirtualKey.LeftMenu) IsDown[VirtualKey.LeftMenu] = false;
                        if (vkCode == (int)VirtualKey.RightMenu) IsDown[VirtualKey.RightMenu] = false;
                        if (vkCode == (int)VirtualKey.LeftShift) IsDown[VirtualKey.LeftShift] = false;
                        if (vkCode == (int)VirtualKey.RightShift) IsDown[VirtualKey.RightShift] = false;

                        var keyEvent = new KeyEvent((VirtualKey)vkCode)
                        {
                            alt = CheckIsDown(VirtualKey.LeftMenu) || CheckIsDown(VirtualKey.RightMenu),
                            shift = CheckIsDown(VirtualKey.LeftShift) || CheckIsDown(VirtualKey.RightShift)
                        };
                        KeyUp?.Invoke(keyEvent);
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

    private static bool CheckIsDown(VirtualKey key)
    {
        return IsDown.ContainsKey(key) && IsDown[key];
    }
}