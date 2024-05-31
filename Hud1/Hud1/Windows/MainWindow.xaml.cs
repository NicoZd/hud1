using Hud1.Helpers;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Interop;

namespace Hud1;

public partial class MainWindow : Window
{
    public static MainWindow? Instance;

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

    public void ActivateWindow()
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