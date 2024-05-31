using System.Windows;
using System.Windows.Interop;

namespace Hud1.Helpers;

internal class MainWindowForegroundRestorer
{
    private readonly bool wasForeground;

    public MainWindowForegroundRestorer()
    {
        var foreground = WindowsAPI.GetForegroundWindow();
        var hwndMain = MainWindow.Instance != null ? new WindowInteropHelper(MainWindow.Instance).Handle : 0;
        wasForeground = foreground == hwndMain;
    }

    internal void Restore()
    {
        if (wasForeground)
            // For some unknown reason ActivateWindow must be called later when SetWindowPosition was called before
            _ = Application.Current.Dispatcher.InvokeAsync(MainWindow.Instance!.ActivateWindow);
    }
}