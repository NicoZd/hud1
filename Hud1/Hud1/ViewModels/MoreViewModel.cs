using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;
using System.Windows;

namespace Hud1.ViewModels;

internal partial class MoreViewModel : ObservableObject
{
    private readonly string[] Styles = ["Green", "Red"];

    internal static readonly MoreViewModel Instance = new();

    private MoreViewModel()
    {
    }

    internal void BuildNavigation()
    {
        var Configure = HudViewModel.Instance.Configure;

        NavigationStates.EXIT.RightAction = Shutdown;
        Configure(NavigationStates.EXIT)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.EXIT.ExecuteRight);

        NavigationStates.HUD_POSITION.Value = UserConfig.Current.HudPosition;
        AssignNextHudPosition(0);
        NavigationStates.HUD_POSITION.LeftAction = SelectHudPos(-1);
        NavigationStates.HUD_POSITION.RightAction = SelectHudPos(1);
        Configure(NavigationStates.HUD_POSITION)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.HUD_POSITION.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.HUD_POSITION.ExecuteRight);

        NavigationStates.DEVELOPER_MODE.Value = UserConfig.Current.DevModeEnabled;
        NavigationStates.DEVELOPER_MODE.LeftAction = EnableTouchMode(false);
        NavigationStates.DEVELOPER_MODE.RightAction = EnableTouchMode(true);
        Configure(NavigationStates.DEVELOPER_MODE)
            .InternalTransition(NavigationTriggers.LEFT, NavigationStates.DEVELOPER_MODE.ExecuteLeft)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.DEVELOPER_MODE.ExecuteRight);

        NavigationStates.STYLE.Value = UserConfig.Current.Style;
        SelectStyle(0);
        NavigationStates.STYLE.LeftAction = PrevStyle;
        NavigationStates.STYLE.RightAction = NextStyle;
        Configure(NavigationStates.STYLE)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.STYLE.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.STYLE.ExecuteRight);

        NavigationStates.FONT.Value = UserConfig.Current.Font;
        SelectFont(0);
        NavigationStates.FONT.LeftAction = PrevFont;
        NavigationStates.FONT.RightAction = NextFont;
        Configure(NavigationStates.FONT)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.FONT.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.FONT.ExecuteRight);

        HudViewModel.Instance.MakeNav(NavigationStates.MENU_MORE, NavigationStates.MORE_VISIBLE,
            [
            NavigationStates.EXIT,
            NavigationStates.CROSSHAIR_MONITOR,
            NavigationStates.HUD_POSITION,
            NavigationStates.DEVELOPER_MODE,
            NavigationStates.STYLE,
            NavigationStates.FONT
            ]);
    }

    private void Shutdown()
    {
        Application.Current.Shutdown();
    }

    private static Action? EnableTouchMode(bool v)
    {
        return () =>
        {
            NavigationStates.DEVELOPER_MODE.Value = v;
        };
    }

    private Action SelectHudPos(int dir)
    {
        return () =>
        {
            AssignNextHudPosition(dir);
        };
    }

    internal void AssignNextHudPosition(int dir)
    {
        var monitors = Monitors.All;
        var monitorCount = monitors.Count;
        string[] positions = ["Left", "Right"];

        List<string> options = [];
        for (var screenIndex = 0; screenIndex < monitorCount; screenIndex++)
        {
            for (var positionIndex = 0; positionIndex < positions.Length; positionIndex++)
            {
                var name = screenIndex + ":" + positions[positionIndex];
                options.Add(name);
            }
        }

        var currentOptionIndex = options.IndexOf((string)NavigationStates.HUD_POSITION.Value);
        if (currentOptionIndex == -1)
        {
            currentOptionIndex = 0;
        }
        else
        {
            currentOptionIndex += dir;
        }
        var newIndex = Math.Min(Math.Max(currentOptionIndex, 0), options.Count - 1);
        var newHudPosition = options[newIndex];

        Console.WriteLine($"AssignNextHudPosition dir = {dir}, {NavigationStates.HUD_POSITION.Value} => {options[newIndex]}");

        NavigationStates.HUD_POSITION.Value = options[newIndex];
    }

    private void NextStyle()
    {
        SelectStyle(1);
    }

    private void PrevStyle()
    {
        SelectStyle(-1);
    }

    internal void SelectStyle(int dir)
    {
        var currentStyleIndex = Array.IndexOf(Styles, NavigationStates.STYLE.Value);
        if (currentStyleIndex == -1)
        {
            currentStyleIndex = 0;
            dir = 0;
        }
        var prevStyleIndex = (currentStyleIndex + dir + Styles.Length) % Styles.Length;
        NavigationStates.STYLE.Value = Styles[prevStyleIndex];
        AppStyle.SelectStyle((string)NavigationStates.STYLE.Value, (string)NavigationStates.FONT.Value);
    }

    private void NextFont()
    {
        SelectFont(1);
    }

    private void PrevFont()
    {
        SelectFont(-1);
    }

    internal void SelectFont(int dir)
    {
        var fonts = HudFonts.GetFonts();
        if (fonts.Count == 0)
        {
            NavigationStates.FONT.Value = "No Font";
            return;
        }

        var currentStyleIndex = fonts.FindIndex(x => x.Name.Equals(NavigationStates.FONT.Value));
        if (currentStyleIndex == -1)
        {
            currentStyleIndex = 0;
            dir = 0;
        }

        var nextStyleIndex = (currentStyleIndex + dir + fonts.Count) % fonts.Count;
        NavigationStates.FONT.Value = fonts[nextStyleIndex].Name;
        AppStyle.SelectStyle((string)NavigationStates.STYLE.Value, (string)NavigationStates.FONT.Value);
    }
}
