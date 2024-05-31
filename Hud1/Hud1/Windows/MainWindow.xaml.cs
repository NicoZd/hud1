﻿using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Models;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

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

    private void OnWindowActivated(object sender, EventArgs e)
    {
        Console.WriteLine("OnWindowActivated");
        MainWindowViewModel.Instance.Active = true;
        MainWindowViewModel.Instance.HudVisibility = Visibility.Visible;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("MainWindow OnWindowLoaded");

        NavigationStates.TOUCH_MODE.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.TOUCH_MODE.SelectionBoolean))
            {
                Debug.Print("TOUCH_MODE {0}", NavigationStates.TOUCH_MODE.SelectionBoolean);

                UpdateTouchMode();
            }
        };
        UpdateTouchMode();
    }

    private void UpdateTouchMode()
    {
        var hwnd = new WindowInteropHelper(this).Handle;

        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);

        var newStyle = NavigationStates.TOUCH_MODE.SelectionBoolean ?
            extendedStyle | WindowsAPI.WS_EX_NOACTIVATE :
            extendedStyle & ~WindowsAPI.WS_EX_NOACTIVATE;

        Debug.Print("UpdateTouchMode {0} {1} {2}", NavigationStates.TOUCH_MODE.SelectionBoolean, extendedStyle, newStyle);

        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE, newStyle);

        if (!NavigationStates.TOUCH_MODE.SelectionBoolean)
        {
            MainWindow.Instance!.ActivateWindow();
        }
    }
}