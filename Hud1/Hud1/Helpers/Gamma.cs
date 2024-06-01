using System.Runtime.InteropServices;

namespace Hud1.Helpers;

public static class Gamma
{
    [DllImport("gdi32.dll")]
    private static extern unsafe bool SetDeviceGammaRamp(int hdc, void* ramp);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateDC(string lpszDriver, string? lpszDevice, string? lpszOutput, IntPtr lpInitData);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(IntPtr hdc);

    public static unsafe bool SetGamma(double gamma)
    {
        var gArray = stackalloc short[3 * 256];

        // just all 
        foreach (var monitor in Monitors.All)
        {
            Console.WriteLine("Set Gamma for DeviceName {0}", monitor.DeviceName);

            var hdc = CreateDC(monitor.DeviceName, null, null, IntPtr.Zero).ToInt32();

            var idx = gArray;
            double offset = 0;
            double range2 = 255;

            var gammas = new double[3];
            gammas[0] = 1 + ((gamma - 1) * 1);
            gammas[1] = 1 + ((gamma - 1) * 1);
            gammas[2] = 1 + ((gamma - 1) * 1);

            for (var j = 0; j < 3; j++)
            {
                for (var i = 0; i < 256; i++)
                {
                    var factor = (i + offset) / range2;
                    factor = Math.Pow(factor, 1 / gammas[j]);
                    var arrayVal = (int)(factor * 0xffff);
                    if (arrayVal > 65535)
                        arrayVal = 65535;
                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            var retVal = SetDeviceGammaRamp(hdc, gArray);
            DeleteDC(hdc);
        }

        return true;
    }
}
