using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace Hud1.Helpers.CustomSplashScreen
{
    [Flags]
    public enum SetWindowPosFlags : uint
    {
        SWP_NOACTIVATE = 0x0010,
        SWP_NOMOVE = 0x0002,
        SWP_NOSIZE = 0x0001
    }

    public class SafeNativeMethods
    {
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int ULW_ALPHA = 0x00000002;
        public const int AC_SRC_OVER = 0x00000000;
        public const int AC_SRC_ALPHA = 0x00000001;
        public static readonly nint HWND_TOP = new nint(0);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst, nint pptDst, nint psize, nint hdcSrc, nint pptSrc, uint crKey, [In] ref BLENDFUNCTION pblend, uint dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


    }

    public sealed class SplashScreenWrapper
    {
        private readonly SplashScreen splashScreen;
        private readonly DispatcherTimer splashForegroundTimer;
        private SafeNativeMethods.BLENDFUNCTION blendFunction;

        public SplashScreenWrapper(string resourceName)
            : this(Assembly.GetEntryAssembly()!, resourceName)
        {
        }

        public SplashScreenWrapper(Assembly resourceAssembly, string resourceName)
        {
            splashScreen = new SplashScreen(resourceAssembly, resourceName);
            splashForegroundTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            splashForegroundTimer.Tick += delegate
            {
                // this brings the splash to the top of the z-order without activating it
                SafeNativeMethods.SetWindowPos(splashScreen.GetHandle(), SafeNativeMethods.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
            };

            blendFunction = new SafeNativeMethods.BLENDFUNCTION
            {
                BlendOp = SafeNativeMethods.AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = 255,
                AlphaFormat = SafeNativeMethods.AC_SRC_ALPHA
            };
        }

        public void Show(bool autoClose)
        {
            // don't use topmost because it's obnoxious...
            splashScreen.Show(autoClose, topMost: false);

            // ...but do make sure the splash appears above all other windows in *our* application (ie. it's topmost within the scope of our application)
            splashForegroundTimer.Start();
        }

        public void Close(TimeSpan fadeoutDuration)
        {
            var fadeDurationTicks = fadeoutDuration.Ticks;
            var remainingTicks = fadeDurationTicks;
            var lastCheckTicks = DateTime.UtcNow.Ticks;
            var dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                // 60fps
                Interval = TimeSpan.FromMilliseconds(1000 / 60d)
            };
            var opacity = 1d;

            // we implement our own fade logic because SplashScreen's logic is flawed (it activates the splash screen)
            dispatcherTimer.Tick += (s, e) =>
            {
                var tickChange = DateTime.UtcNow.Ticks - lastCheckTicks;
                lastCheckTicks = DateTime.UtcNow.Ticks;
                remainingTicks -= tickChange;
                remainingTicks = Math.Max(0, remainingTicks);
                opacity = (double)remainingTicks / fadeDurationTicks;

                if (remainingTicks == 0)
                {
                    // finished fading
                    splashScreen.Close(TimeSpan.Zero);
                    dispatcherTimer.Stop();
                    splashForegroundTimer.Stop();
                }
                else
                {
                    // still fading
                    blendFunction.SourceConstantAlpha = (byte)(255 * opacity);
                    SafeNativeMethods.UpdateLayeredWindow(splashScreen.GetHandle(), nint.Zero, nint.Zero, nint.Zero, nint.Zero, nint.Zero, 0, ref blendFunction, SafeNativeMethods.ULW_ALPHA);
                }
            };
            dispatcherTimer.Start();
        }


    }
}
