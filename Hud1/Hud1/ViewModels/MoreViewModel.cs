using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers.ScreenHelper;
using Hud1.Models;
using Hud1.Start;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels;

public partial class MoreViewModel : ObservableObject
{
    private readonly string[] Styles = ["Green", "Red"];

    public static readonly MoreViewModel Instance = new();

    [ObservableProperty]
    private string _hudPosition = "0:Right";

    [ObservableProperty]
    private string _hudAlignment = "Right";

    private MoreViewModel()
    {
        _hudPosition = UserConfig.Current.HudPosition;
        ComputeNextHudPosition(0);
    }

    public void BuildNavigation()
    {
        var Navigation = NavigationViewModel.Instance.Navigation;

        NavigationStates.EXIT.RightAction = Shutdown;
        Navigation.Configure(NavigationStates.EXIT)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.EXIT.ExecuteRight);

        NavigationStates.ACTIVATE.RightAction = Activate;
        Navigation.Configure(NavigationStates.ACTIVATE)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.ACTIVATE.ExecuteRight);

        NavigationStates.HUD_POSITION.LeftAction = SelectHudPos(-1);
        NavigationStates.HUD_POSITION.RightAction = SelectHudPos(1);
        Navigation.Configure(NavigationStates.HUD_POSITION)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.HUD_POSITION.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.HUD_POSITION.ExecuteRight);

        NavigationStates.TOUCH_MODE.LeftAction = EnableCursorNav(false);
        NavigationStates.TOUCH_MODE.RightAction = EnableCursorNav(true);
        Navigation.Configure(NavigationStates.TOUCH_MODE)
            .InternalTransition(NavigationTriggers.LEFT, NavigationStates.TOUCH_MODE.ExecuteLeft)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.TOUCH_MODE.ExecuteRight);

        NavigationStates.STYLE.LeftAction = PrevStyle;
        NavigationStates.STYLE.RightAction = NextStyle;
        Navigation.Configure(NavigationStates.STYLE)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.STYLE.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.STYLE.ExecuteRight);

        NavigationStates.FONT.LeftAction = PrevFont;
        NavigationStates.FONT.RightAction = NextFont;
        Navigation.Configure(NavigationStates.FONT)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.FONT.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.FONT.ExecuteRight);

        NavigationViewModel.MakeNav(NavigationStates.MENU_MORE, NavigationStates.MORE_VISIBLE,
            [NavigationStates.EXIT, NavigationStates.HUD_POSITION,
            NavigationStates.TOUCH_MODE, NavigationStates.STYLE, NavigationStates.FONT]);
    }

    private void Shutdown()
    {
        Application.Current.Shutdown();
    }

    private static Action? EnableCursorNav(bool v)
    {
        return () =>
        {
            NavigationStates.TOUCH_MODE.SelectionBoolean = v;
        };
    }

    private Action SelectHudPos(int dir)
    {
        return () =>
        {
            ComputeNextHudPosition(dir);
        };
    }

    private void ComputeNextHudPosition(int dir)
    {
        Console.WriteLine("Compute Hud Pos {0}", dir);
        var screenCount = Screen.AllScreens.Count();
        string[] positions = ["Left", "Right"];

        List<string> options = [];
        for (var screenIndex = 0; screenIndex < screenCount; screenIndex++)
        {
            for (var positionIndex = 0; positionIndex < positions.Length; positionIndex++)
            {
                var name = screenIndex + ":" + positions[positionIndex];
                options.Add(name);
                Console.WriteLine("perm {0}", name);
            }
        }

        var currentOptionIndex = options.IndexOf(HudPosition);
        if (currentOptionIndex == -1)
        {
            currentOptionIndex = 0;
        }
        else
        {
            currentOptionIndex += dir;
        }
        var newIndex = Math.Min(Math.Max(currentOptionIndex, 0), options.Count - 1);

        HudPosition = options[newIndex];
        HudAlignment = HudPosition.Split(":")[1];

        NavigationStates.HUD_POSITION.SelectionLabel = "Display " + HudPosition.Split(":")[0] + ", " + HudAlignment;
    }

    private void Activate()
    {
        Console.WriteLine("Activate");
        MainWindowViewModel.Instance?.Activate();
    }

    private void NextStyle()
    {
        SelectStyle(1);

    }

    private void PrevStyle()
    {
        SelectStyle(-1);
    }

    public void SelectStyle(int dir)
    {
        var currentStyleIndex = Array.IndexOf(Styles, NavigationStates.STYLE.SelectionLabel);
        if (currentStyleIndex == -1)
        {
            currentStyleIndex = 0;
            dir = 0;
        }
        var prevStyleIndex = (currentStyleIndex + dir + Styles.Length) % Styles.Length;
        NavigationStates.STYLE.SelectionLabel = Styles[prevStyleIndex];
        App.SelectStyle(NavigationStates.STYLE.SelectionLabel, NavigationStates.FONT.SelectionLabel);
    }

    private string[] FontList()
    {
        var fontsFolder = Path.Combine(Start.Setup.VersionPath, "Fonts");
        if (!Directory.Exists(fontsFolder))
            return [];
        var fileEntries = Directory.GetFiles(fontsFolder, "*.*").Where(s => s.ToLower().EndsWith(".ttf") || s.ToLower().EndsWith(".otf")).ToArray();
        List<string> fonts = [];
        for (var i = 0; i < fileEntries.Length; i++)
        {
            var ff = Fonts.GetFontFamilies(fileEntries[i]);
            if (ff.Count > 0)
            {
                var y = ff.First();
                var k = y.Source.Split("#");
                var v = k[^1];
                Console.WriteLine("fontCol.Families[0].Name {0}", v);
                fonts.Add(v);
            }
        }
        return [.. fonts];
    }

    private void NextFont()
    {
        SelectFont(1);
    }

    private void PrevFont()
    {
        SelectFont(-1);
    }

    public void SelectFont(int dir)
    {
        var Fonts = FontList();
        if (Fonts.Length == 0) return;
        var currentStyleIndex = Array.IndexOf(Fonts, NavigationStates.FONT.SelectionLabel);
        if (currentStyleIndex == -1)
        {
            currentStyleIndex = 0;
            dir = 0;
        }
        var prevStyleIndex = (currentStyleIndex + dir + Fonts.Length) % Fonts.Length;
        NavigationStates.FONT.SelectionLabel = Fonts[prevStyleIndex];
        App.SelectStyle(NavigationStates.STYLE.SelectionLabel, NavigationStates.FONT.SelectionLabel);
    }
}
