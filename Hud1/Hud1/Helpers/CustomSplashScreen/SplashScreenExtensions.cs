using System.Reflection;
using System.Windows;

namespace Hud1.Helpers.CustomSplashScreen
{

    internal static class SplashScreenExtensions
    {
        public static nint GetHandle(this SplashScreen @this)
        {
            return (nint)typeof(SplashScreen).GetField("_hwnd", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(@this)!;
        }
    }
}
