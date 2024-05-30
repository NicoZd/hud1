using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Runtime.InteropServices;
using System.Windows;

namespace Hud1.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public static readonly MainWindowViewModel Instance = new();

    [ObservableProperty]
    public bool _active = true;

    [ObservableProperty]
    public bool _isForeground = false;

    [ObservableProperty]
    public Visibility _hudVisibility = Visibility.Visible;

    internal nint Hwnd;
    private readonly WinEventDelegate winEventDelegate;

    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);


    private const uint EVENT_SYSTEM_FOREGROUND = 3;
    private const int WINEVENT_OUTOFCONTEXT = 0;

    private MainWindowViewModel()
    {
        winEventDelegate = new WinEventDelegate(WinEventProc);
    }

    internal void InitWindow(nint hwnd)
    {
        Hwnd = hwnd;
        SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        ComputeIsForeground();

        var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler((_, _) =>
        {
            ComputeIsForeground();
        });
        dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        dispatcherTimer.Start();
    }

    public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        ComputeIsForeground();
    }

    private void ComputeIsForeground()
    {
        var foreground = WindowsAPI.GetForegroundWindow();
        IsForeground = (foreground == Hwnd) || WindowsAPI.IsMouseHidden();
    }

    public void ActivateWindow()
    {
        var threadId1 = WindowsAPI.GetWindowThreadProcessId(WindowsAPI.GetForegroundWindow(), IntPtr.Zero);
        var threadId2 = WindowsAPI.GetWindowThreadProcessId(Hwnd, IntPtr.Zero);

        if (threadId1 != threadId2)
        {
            WindowsAPI.AttachThreadInput(threadId1, threadId2, true);
            WindowsAPI.SetForegroundWindow(Hwnd);
            WindowsAPI.AttachThreadInput(threadId1, threadId2, false);
        }
        else
        {
            WindowsAPI.SetForegroundWindow(Hwnd);
        }
    }

    internal void HandleKeyActivator()
    {
        Console.WriteLine("HandleKeyActivator");

        if (Active)
        {
            HudVisibility = Visibility.Collapsed;
            Active = false;
        }
        else
        {
            HudVisibility = Visibility.Visible;
            Active = true;

        }
    }

}
