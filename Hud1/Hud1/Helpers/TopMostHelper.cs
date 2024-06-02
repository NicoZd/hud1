namespace Hud1.Helpers;

internal class TopMostHelper
{
    internal static readonly TopMostHelper Instance = new();

    internal List<nint> TopWindows = [];

    private TopMostHelper()
    {
        var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler((_, _) =>
        {
            Run();
        });
        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        dispatcherTimer.Start();
    }

    private void Run()
    {
        //Debug.Print($"TopMostHelper Run {TopWindows.Count}");
        TopWindows.ForEach(hwnd =>
        {
            //Debug.Print($"MakeTopMost {hwnd}");
            WindowsAPI.SetWindowPos(hwnd, WindowConstants.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
        });
    }

}
