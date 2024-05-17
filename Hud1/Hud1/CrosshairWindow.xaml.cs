using Hud1.Helpers;
using System.Windows;
using System.Windows.Interop;

namespace Hud1
{
    public partial class CrosshairWindow : Window
    {
        public CrosshairWindow()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("OnWindowLoaded {0}");
            var hwnd = new WindowInteropHelper(this).Handle;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((_, _) =>
            {
                WindowsAPI.SetWindowPos(hwnd, WindowsAPI.HWND_TOP, 0, 0, 0, 0, WindowsAPI.SetWindowPosFlags.SWP_NOMOVE | WindowsAPI.SetWindowPosFlags.SWP_NOSIZE | WindowsAPI.SetWindowPosFlags.SWP_NOACTIVATE);
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
            WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
                extendedStyle
                | WindowsAPI.WS_EX_NOACTIVATE
                | WindowsAPI.WS_EX_TRANSPARENT
                );

            int width = (int)SystemParameters.PrimaryScreenWidth;
            int height = (int)SystemParameters.PrimaryScreenHeight;

            Console.WriteLine("Window_Loaded {0} {1}", width, height);

            this.Width = width;
            this.Height = height;
            this.Left = 0;
            this.Top = 0;
        }
    }
}
