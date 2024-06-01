using Hud1.Helpers;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Windows;

public partial class MainWindow : Window
{
    internal static MainWindow? Instance;

    internal static void Create()
    {
        Instance = new MainWindow();
        Instance.Show();
        Application.Current.MainWindow = Instance;
    }

    private MainWindow()
    {
        InitializeComponent();
        CrosshairWindow.Create();
    }

    internal void ActivateWindow()
    {
        var hwnd = new WindowInteropHelper(this).Handle;

        var threadId1 = WindowsAPI.GetWindowThreadProcessId(WindowsAPI.GetForegroundWindow(), IntPtr.Zero);
        var threadId2 = WindowsAPI.GetWindowThreadProcessId(hwnd, IntPtr.Zero);

        if (threadId1 != threadId2)
        {
            WindowsAPI.AttachThreadInput(threadId1, threadId2, true);
            WindowsAPI.SetForegroundWindow(hwnd);
            WindowsAPI.AttachThreadInput(threadId1, threadId2, false);
        }
        else
        {
            WindowsAPI.SetForegroundWindow(hwnd);
        }
    }
}