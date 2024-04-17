﻿namespace SplashTest
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Threading;
    using WPF = System.Windows;

    public sealed class SplashScreen
    {
        private readonly WPF.SplashScreen splashScreen;
        private readonly DispatcherTimer splashForegroundTimer;
        private SafeNativeMethods.BLENDFUNCTION blendFunction;

        public SplashScreen(string resourceName)
            : this(Assembly.GetEntryAssembly(), resourceName)
        {
        }

        public SplashScreen(Assembly resourceAssembly, string resourceName)
        {
            this.splashScreen = new WPF.SplashScreen(resourceAssembly, resourceName);
            this.splashForegroundTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            this.splashForegroundTimer.Tick += delegate
            {
                // this brings the splash to the top of the z-order without activating it
                SafeNativeMethods.SetWindowPos(this.splashScreen.GetHandle(), SafeNativeMethods.HWND_TOP, 0, 0, 0, 0, SafeNativeMethods.SetWindowPosFlags.SWP_NOMOVE | SafeNativeMethods.SetWindowPosFlags.SWP_NOSIZE | SafeNativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);
            };

            this.blendFunction = new SafeNativeMethods.BLENDFUNCTION
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
            this.splashScreen.Show(autoClose, topMost: false);

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
                    this.splashScreen.Close(TimeSpan.Zero);
                    dispatcherTimer.Stop();
                    splashForegroundTimer.Stop();
                }
                else
                {
                    // still fading
                    this.blendFunction.SourceConstantAlpha = (byte)(255 * opacity);
                    SafeNativeMethods.UpdateLayeredWindow(this.splashScreen.GetHandle(), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref blendFunction, SafeNativeMethods.ULW_ALPHA);
                }
            };
            dispatcherTimer.Start();
        }

        private sealed class SafeNativeMethods
        {
            public const int WS_EX_NOACTIVATE = 0x08000000;
            public const int ULW_ALPHA = 0x00000002;
            public const int AC_SRC_OVER = 0x00000000;
            public const int AC_SRC_ALPHA = 0x00000001;
            public static readonly IntPtr HWND_TOP = new IntPtr(0);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, IntPtr psize, IntPtr hdcSrc, IntPtr pptSrc, uint crKey, [In] ref BLENDFUNCTION pblend, uint dwFlags);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

            public struct BLENDFUNCTION
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;
            }

            [Flags]
            public enum SetWindowPosFlags : uint
            {
                SWP_NOACTIVATE = 0x0010,
                SWP_NOMOVE = 0x0002,
                SWP_NOSIZE = 0x0001
            }
        }
    }
}
