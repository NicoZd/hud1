using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hud1.Helpers;

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MSLLHOOKSTRUCT
{
    public POINT pt;
    public uint mouseData;
    public uint flags;
    public uint time;
    public IntPtr dwExtraInfo;

}

internal static class GlobalMouseHook
{
    internal delegate void MouseDownHandler(int button, POINT point);
    internal static event MouseDownHandler? MouseDown;

    internal delegate void MouseUpHandler(int button, POINT point);
    internal static event MouseUpHandler? MouseUp;

    private static IntPtr HookID = IntPtr.Zero;

    internal static int HIWORD(int n)
    {
        return (n >> 16) & 0xffff;
    }

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
            //Debug.Print($"code: {code} {wParam}");
            try
            {

                switch (wParam)
                {
                    case WindowMessage.WM_LBUTTONDOWN:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            MouseDown?.Invoke(1, hookStruct.pt);
                            break;
                        }
                    case WindowMessage.WM_RBUTTONDOWN:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            MouseDown?.Invoke(2, hookStruct.pt);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONDOWN:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            MouseDown?.Invoke(3, hookStruct.pt);
                            break;
                        }

                    case WindowMessage.WM_LBUTTONUP:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            MouseUp?.Invoke(1, hookStruct.pt);
                            break;
                        }
                    case WindowMessage.WM_RBUTTONUP:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            MouseUp?.Invoke(2, hookStruct.pt);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONUP:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            MouseUp?.Invoke(3, hookStruct.pt);
                            break;
                        }
                    case WindowMessage.WM_XBUTTONDOWN:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            Debug.Print($"{hookStruct.pt.x}/{hookStruct.pt.y}");
                            int buttonCode = HIWORD((int)hookStruct.mouseData);
                            MouseDown?.Invoke(3 + buttonCode, hookStruct.pt);
                            break;
                        }
                    case WindowMessage.WM_XBUTTONUP:
                        {
                            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                            int buttonCode = HIWORD((int)hookStruct.mouseData);
                            MouseUp?.Invoke(3 + buttonCode, hookStruct.pt);
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