namespace Hud1.Service.SplashScreen
{
    using System;
    using System.Reflection;
    using WPF = System.Windows;

    internal static class SplashScreenExtensions
    {
        public static nint GetHandle(this WPF.SplashScreen @this)
        {
            return (nint)typeof(WPF.SplashScreen).GetField("_hwnd", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(@this)!;
        }
    }
}
