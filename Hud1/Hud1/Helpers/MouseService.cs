using System.Runtime.InteropServices;

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
            IgnoreNextEvent = true;
            mouse_event(((int)button), 0, 0, 0, 0);
            IgnoreNextEvent = false;
        }

        public static void MouseUp(MouseButton button)
        {
            IgnoreNextEvent = true;
            mouse_event(((int)button) * 2, 0, 0, 0, 0);
            IgnoreNextEvent = false;
        }

    }
}
