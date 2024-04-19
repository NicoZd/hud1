namespace SplashTest
{
    using System;
    using System.Reflection;
    using WPF = System.Windows;

    internal static class SplashScreenExtensions
    {
        public static IntPtr GetHandle(this WPF.SplashScreen @this)
        {
            return (IntPtr)typeof(WPF.SplashScreen).GetField("_hwnd", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(@this);
        }
    }
}
