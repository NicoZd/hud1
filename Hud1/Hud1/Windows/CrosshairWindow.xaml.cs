﻿using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Windows;

public partial class CrosshairWindow : Window
{
    internal static CrosshairWindow? Instance;

    internal static void Create()
    {
        Instance = new CrosshairWindow();
        Instance.Show();
    }

    private CrosshairWindow()
    {
        Opacity = 0;
        InitializeComponent();
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;

        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowConstants.GWL_EXSTYLE);
        WindowsAPI.SetWindowLong(hwnd, WindowConstants.GWL_EXSTYLE,
            extendedStyle
            | WindowConstants.WS_EX_NOACTIVATE
            | WindowConstants.WS_EX_TRANSPARENT
            );

        NavigationStates.CROSSHAIR_ENABLED.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_FORM.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_COLOR.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_OPACITY.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_SIZE.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_OUTLINE.PropertyChanged += UpdateCrosshair;
    }

    private void UpdateCrosshair(object? sender, PropertyChangedEventArgs e)
    {
        string[] validProperties = [nameof(NavigationState.SelectionLabel), nameof(NavigationState.SelectionBoolean)];
        if (validProperties.Contains(e.PropertyName))
        {
            Debug.Print($"UpdateCrosshair based on Prop: {e.PropertyName}");
            CrosshairViewModel.Instance.Redraw();
        }
    }

}
