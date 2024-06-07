namespace Hud1.Helpers;

internal static class Gamma
{
    internal static unsafe bool SetGamma(double gamma)
    {
        var gArray = stackalloc short[3 * 256];

        // just all 
        foreach (var monitor in Monitors.All)
        {
            Console.WriteLine("Set Gamma for DeviceName {0}", monitor.DeviceName);

            var hdc = WindowsAPI.CreateDC(monitor.DeviceName, null, null, IntPtr.Zero).ToInt32();

            var idx = gArray;

            for (var j = 0; j < 3; j++)
            {
                for (var i = 0; i < 256; i++)
                {
                    var factor = Math.Pow(i / 255.0, 1.0 / gamma);
                    var arrayVal = (int)(factor * 0xffff);
                    if (arrayVal > 65535)
                        arrayVal = 65535;
                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            var retVal = WindowsAPI.SetDeviceGammaRamp(hdc, gArray);
            WindowsAPI.DeleteDC(hdc);
        }

        return true;
    }
}
