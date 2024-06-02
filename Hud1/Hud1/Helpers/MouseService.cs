using System.Windows;

namespace Hud1.Helpers;

internal class MouseService
{
    internal static bool IgnoreNextEvent { get; set; }

    internal enum MouseButton
    {
        Left = 0x2,
        Right = 0x8,
        Middle = 0x20
    }

    internal static void MouseDown(MouseButton button)
    {
        Console.WriteLine("MouseDown1");
        IgnoreNextEvent = true;
        WindowsAPI.mouse_event((int)button, 0, 0, 0, 0);

        ThreadPool.QueueUserWorkItem((_) =>
        {
            Thread.Sleep(1);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IgnoreNextEvent = false;
                Console.WriteLine("MouseDown2");
            }));
        });
    }

    internal static void MouseUp(MouseButton button)
    {
        Console.WriteLine("MouseUp1");
        IgnoreNextEvent = true;
        WindowsAPI.mouse_event(((int)button) * 2, 0, 0, 0, 0);

        ThreadPool.QueueUserWorkItem((_) =>
        {
            Thread.Sleep(1);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IgnoreNextEvent = false;
                Console.WriteLine("MouseUp2");
            }));

        });
    }
}
