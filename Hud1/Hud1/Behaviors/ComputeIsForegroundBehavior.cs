using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Interop;
using static Hud1.Helpers.WindowsAPI;

namespace Hud1.Behaviors;

internal class ComputeIsForegroundBehavior : Behavior<Window>
{
    private readonly WinEventDelegate winEventDelegate;
    private nint hwnd;

    public ComputeIsForegroundBehavior()
    {
        winEventDelegate = new WindowsAPI.WinEventDelegate(WinEventProc);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        hwnd = new WindowInteropHelper(AssociatedObject).Handle;
        WindowsAPI.SetWinEventHook(WindowConstants.EVENT_SYSTEM_FOREGROUND, WindowConstants.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, winEventDelegate, 0, 0, WindowConstants.WINEVENT_OUTOFCONTEXT);

        var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler((_, _) =>
        {
            ComputeIsForeground();
        });
        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(250);
        dispatcherTimer.Start();

        ComputeIsForeground();
    }

    private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        ComputeIsForeground();
    }

    private void ComputeIsForeground()
    {
        var foreground = WindowsAPI.GetForegroundWindow();
        var isForeground = (foreground == hwnd) || !(bool)NavigationStates.DEVELOPER_MODE.Value;
        var isMouseHidden = WindowsAPI.IsMouseHidden();
        var newIsForeground = isForeground || isMouseHidden;
        if (newIsForeground != MainWindowViewModel.Instance.IsForeground)
        {
            // Debug.Print($"ComputeIsForeground isForeground:{isForeground} isMouseHidden:{isMouseHidden} newIsForeground:{newIsForeground}");
            MainWindowViewModel.Instance.IsForeground = newIsForeground;
        }
    }
}
