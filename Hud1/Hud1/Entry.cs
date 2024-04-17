namespace SplashTest
{
    using Hud1;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    class Entry
    {
        private static SplashScreen splashScreen;
        private static App app;

        [STAThread]
        public static void Main(string[] args)
        {
            var showSplash = !args.Any(x => string.Equals("-nosplash", x, StringComparison.OrdinalIgnoreCase));

            if (showSplash)
            {
                ShowSplashScreen();
            }

            CreateApp();

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
                    CloseSplashScreen(fadeDuration: TimeSpan.FromMilliseconds(150));
                };
                splashTimer.Start();
            }

            RunApp();
        }

        private static void ShowSplashScreen()
        {
            splashScreen = new SplashScreen(resourceName: "/Images/Abstract-colorful-fluid-background-on-transparent-PNG.png");
            splashScreen.Show(autoClose: false);
        }

        private static void CloseSplashScreen(TimeSpan fadeDuration)
        {
            splashScreen.Close(fadeDuration);
        }

        private static void CreateApp()
        {
            app = new App();
            app.InitializeComponent();
        }

        private static void RunApp()
        {
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
