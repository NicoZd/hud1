using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace Hud1.Helpers
{
    public class MouseService
    {
        public static bool IgnoreNextEvent { get; set; }

        public enum MouseButton
        {
            Left = 0x2,
            Right = 0x8,
            Middle = 0x20
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        public static void MouseDown(MouseButton button)
        {
            Console.WriteLine("MouseDown1");
            IgnoreNextEvent = true;
            mouse_event(((int)button), 0, 0, 0, 0);

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

        public static void MouseUp(MouseButton button)
        {
            Console.WriteLine("MouseUp1");
            IgnoreNextEvent = true;
            mouse_event(((int)button) * 2, 0, 0, 0, 0);

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
}
