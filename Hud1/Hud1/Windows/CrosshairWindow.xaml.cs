using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Models;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace Hud1;

public partial class CrosshairWindow : Window
{
    private string lastRedrawScreenConfig = "";

    public CrosshairWindow()
    {
        InitializeComponent();
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;

        var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler((_, _) =>
        {
            WindowsAPI.SetWindowPos(hwnd, WindowsAPI.HWND_TOP, 0, 0, 0, 0, WindowsAPI.SetWindowPosFlags.SWP_NOMOVE | WindowsAPI.SetWindowPosFlags.SWP_NOSIZE | WindowsAPI.SetWindowPosFlags.SWP_NOACTIVATE);
#if HOT
            Redraw(force: true);
#else
            Redraw(force: false);
#endif
        });
        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        dispatcherTimer.Start();

        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
            extendedStyle
            | WindowsAPI.WS_EX_NOACTIVATE
            | WindowsAPI.WS_EX_TRANSPARENT
            );

        this.SetWindowPosition(WindowPositions.Maximize, Screen.AllScreens.ElementAt(0));

        Redraw(force: true);

        NavigationStates.CROSSHAIR_ENABLED.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_DISPLAY.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_FORM.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_COLOR.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_OPACITY.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_SIZE.PropertyChanged += UpdateCrosshair;
        NavigationStates.CROSSHAIR_OUTLINE.PropertyChanged += UpdateCrosshair;
    }

    private void Redraw(bool force)
    {
        // current screen
        var screens = Screen.AllScreens;
        var display = int.Parse(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);

        if (screens.Count() - 1 < display)
        {
            NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel = "0";
            display = 0;
            Debug.Print("RESET to display 0");
        }
        var screen = screens.ElementAt(display);

        var currentRedrawConfig = screen.ScaleFactor + " " + screen.Bounds.Width + " " + screen.Bounds.Height + " " + display;

        if (currentRedrawConfig != lastRedrawScreenConfig || force)
        {
            Debug.Print("Apply");

            lastRedrawScreenConfig = currentRedrawConfig;
            this.SetWindowPosition(WindowPositions.Maximize, screen);
            CrosshairViewModel.Instance.Redraw(PART_Container);
        }
    }

    private void UpdateCrosshair(object? sender, PropertyChangedEventArgs e)
    {
        string[] validProperties = [nameof(NavigationState.SelectionLabel), nameof(NavigationState.SelectionBoolean)];
        if (validProperties.Contains(e.PropertyName))
        {
            Debug.Print($"UpdateCrosshair based on Prop: {e.PropertyName}");
            Redraw(force: true);
        }

    }

}
