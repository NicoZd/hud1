namespace Hud1.Service.SplashScreen
{
    using Hud1;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    class Entry
    {
        private static readonly SplashScreen splashScreen = new(resourceName: "/Resources/fluid-background-transparent.png");

        private static readonly App app = new();

        [STAThread]
        public static void Main(string[] args)
        {
            var showSplash = !args.Any(x => string.Equals("-nosplash", x, StringComparison.OrdinalIgnoreCase));

            if (showSplash)
            {
                splashScreen.Show(autoClose: false);
            }

            app.InitializeComponent();

            if (showSplash)
            {
                // pump until loaded
                PumpDispatcherUntilPriority(DispatcherPriority.Loaded);

                // start a timer, after which the splash can be closed
                var splashTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(0.1)
                };
                splashTimer.Tick += (s, e) =>
                {
                    splashTimer.Stop();
                    splashScreen.Close(TimeSpan.FromMilliseconds(150));
                };
                splashTimer.Start();
            }

            app.Run();
        }
        private static void PumpDispatcherUntilPriority(DispatcherPriority dispatcherPriority)
        {
            var dispatcherFrame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke((ThreadStart)(() => dispatcherFrame.Continue = false), dispatcherPriority);
            Dispatcher.PushFrame(dispatcherFrame);
        }
    }
}
