﻿using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using WpfScreenHelper;

namespace Hud1.Helpers;

public static class Gamma
{
    [DllImport("gdi32.dll")]
    private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateDC(string lpszDriver, string? lpszDevice, string? lpszOutput, IntPtr lpInitData);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(IntPtr hdc);


    [DllImport("user32.dll")]
    static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VGACompatible = 0x10,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    public static unsafe bool SetGamma(double gamma)
    {
        short* gArray = stackalloc short[3 * 256];

        // just all 
        foreach (var screen in Screen.AllScreens)
        {
            Console.WriteLine("Set Gamma for DeviceName {0}", screen.DeviceName);

            Int32 hdc = CreateDC(screen.DeviceName, null, null, IntPtr.Zero).ToInt32();

            short* idx = gArray;
            double offset = 0;
            double range2 = 255;

            double[] gammas = new double[3];
            gammas[0] = 1 + (gamma - 1) * 1;
            gammas[1] = 1 + (gamma - 1) * 1;
            gammas[2] = 1 + (gamma - 1) * 1;

            for (int j = 0; j < 3; j++)
            {
                for (var i = 0; i < 256; i++)
                {
                    var factor = (i + offset) / range2;
                    factor = Math.Pow(factor, 1 / (gammas[j]));
                    int arrayVal = (int)(factor * 0xffff);
                    if (arrayVal > 65535)
                        arrayVal = 65535;
                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            bool retVal = SetDeviceGammaRamp(hdc, gArray);
            DeleteDC(hdc);
        }

        return true;
    }
}
