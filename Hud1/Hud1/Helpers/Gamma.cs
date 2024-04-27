using System.Drawing;
using System.Runtime.InteropServices;
namespace Hud1.Helpers
{
    public static class Gamma
    {
        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);

        public static unsafe bool SetGamma(double gamma)
        {
            var hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();

            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;

            double offset = 0;
            double range2 = 255;

            for (int j = 0; j < 3; j++)
            {
                for (var i = 0; i < 256; i++)
                {
                    var factor = ((double)i + offset) / range2;
                    factor = Math.Pow(factor, 1 / gamma);
                    int arrayVal = (int)(factor * 0xffff);
                    if (arrayVal > 65535)
                        arrayVal = 65535;
                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            bool retVal = SetDeviceGammaRamp(hdc, gArray);

            return true;
        }
    }
}
