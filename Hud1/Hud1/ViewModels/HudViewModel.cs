using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.Models;
using Stateless.Graph;
using Stateless.Reflection;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Hud1.ViewModels
{
    public partial class HudViewModel : ObservableObject
    {
        [ObservableProperty]
        public NavigationState? state;

        [ObservableProperty]
        public Dictionary<string, NavigationState> states = new Dictionary<string, NavigationState> { };

        [ObservableProperty]
        public GammaViewModel gammaViewModel = new();

        [ObservableProperty]
        public MacrosViewModel macrosViewModel = new();

        public Stateless.StateMachine<NavigationState, NavigationTrigger> Navigation;

        private readonly string[] Styles = ["Green", "Red"];

        public static HudViewModel? Instance;
        public static NavigationState? DirectNavigationStateTarget;

        public HudViewModel()
        {
            Instance = this;
            CreateNavigation();

            //string graph = UmlDotGraph.Format(Navigation.GetInfo());
            //Console.WriteLine(graph);

            MacrosViewModel.Navigation = Navigation;

            UpdateModelFromMavigation();
        }

        private void CreateNavigation()
        {
            Navigation = new(NavigationStates.MENU_DISPLAY);

            Navigation.Configure(NavigationStates.ALL)
                .PermitDynamic(NavigationTriggers.DIRECT, () => { return DirectNavigationStateTarget!; });

            Navigation.Configure(NavigationStates.DISPLAY_VISIBLE).SubstateOf(NavigationStates.ALL);
            Navigation.Configure(NavigationStates.MACRO_VISIBLE).SubstateOf(NavigationStates.ALL);
            Navigation.Configure(NavigationStates.CROSSHAIR_VISIBLE).SubstateOf(NavigationStates.ALL);
            Navigation.Configure(NavigationStates.MORE_VISIBLE).SubstateOf(NavigationStates.ALL);

            // MENU
            Navigation.Configure(NavigationStates.MENU_DISPLAY)
                .SubstateOf(NavigationStates.DISPLAY_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.DOWN, NavigationStates.GAMMA)
                .Permit(NavigationTriggers.UP, NavigationStates.GAMMA);

            Navigation.Configure(NavigationStates.MENU_MACRO)
                .SubstateOf(NavigationStates.MACRO_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_DISPLAY)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_CROSSHAIR)
                .Permit(NavigationTriggers.UP, NavigationStates.MACROS_FOLDER)
                .Permit(NavigationTriggers.DOWN, NavigationStates.MACROS);

            Navigation.Configure(NavigationStates.MENU_CROSSHAIR)
                .SubstateOf(NavigationStates.CROSSHAIR_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MORE);

            Navigation.Configure(NavigationStates.MENU_MORE)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_CROSSHAIR)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_DISPLAY)
                .Permit(NavigationTriggers.DOWN, NavigationStates.ACTIVATE)
                .Permit(NavigationTriggers.UP, NavigationStates.FONT);

            // DISPLAY
            NavigationStates.GAMMA.LeftAction = GammaViewModel.SelectPrevGama;
            NavigationStates.GAMMA.RightAction = GammaViewModel.SelectNextGama;
            Navigation.Configure(NavigationStates.GAMMA)
               .SubstateOf(NavigationStates.DISPLAY_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.MENU_DISPLAY)
               .Permit(NavigationTriggers.DOWN, NavigationStates.MENU_DISPLAY)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.GAMMA.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.GAMMA.ExecuteRight);

            // MACROS
            Navigation.Configure(NavigationStates.MACROS)
             .SubstateOf(NavigationStates.MACRO_VISIBLE)
             .OnEntryFrom(NavigationTriggers.UP, MacrosViewModel.OnEntryFromBottom)
             .OnEntryFrom(NavigationTriggers.DOWN, MacrosViewModel.OnEntryFromTop)
             .OnEntry(MacrosViewModel.OnEntry)
             .OnExit(MacrosViewModel.OnExit)
             .InternalTransition(NavigationTriggers.LEFT, MacrosViewModel.OnLeft)
             .InternalTransition(NavigationTriggers.RIGHT, MacrosViewModel.OnRight)
             .InternalTransition(NavigationTriggers.UP, MacrosViewModel.OnUp)
             .InternalTransition(NavigationTriggers.DOWN, MacrosViewModel.OnDown)
             .Permit(NavigationTriggers.RETURN_UP, NavigationStates.MENU_MACRO)
             .Permit(NavigationTriggers.RETURN_DOWN, NavigationStates.MACROS_FOLDER);

            NavigationStates.MACROS_FOLDER.RightAction = () =>
            {
                Console.WriteLine("OPEN {0}", MacrosViewModel._path);
                Process.Start("explorer.exe", MacrosViewModel._path);
            };

            Navigation.Configure(NavigationStates.MACROS_FOLDER)
                .SubstateOf(NavigationStates.MACRO_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.MACROS)
                .Permit(NavigationTriggers.DOWN, NavigationStates.MENU_MACRO)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.MACROS_FOLDER.ExecuteRight);

            // MORE

            NavigationStates.ACTIVATE.RightAction = Activate;
            Navigation.Configure(NavigationStates.ACTIVATE)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.DOWN, NavigationStates.EXIT)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.ACTIVATE.ExecuteRight);

            NavigationStates.EXIT.RightAction = Application.Current.Shutdown;
            Navigation.Configure(NavigationStates.EXIT)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.ACTIVATE)
                .Permit(NavigationTriggers.DOWN, NavigationStates.HUD_POSITION)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.EXIT.ExecuteRight);

            Navigation.Configure(NavigationStates.HUD_POSITION)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.EXIT)
                .Permit(NavigationTriggers.DOWN, NavigationStates.KEYBOARD_CONTROL);

            Navigation.Configure(NavigationStates.KEYBOARD_CONTROL)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.HUD_POSITION)
                .Permit(NavigationTriggers.DOWN, NavigationStates.STYLE)
                .InternalTransition(NavigationTriggers.LEFT, NavigationStates.KEYBOARD_CONTROL.ExecuteLeft)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.KEYBOARD_CONTROL.ExecuteRight);

            NavigationStates.STYLE.LeftAction = PrevStyle;
            NavigationStates.STYLE.RightAction = NextStyle;
            Navigation.Configure(NavigationStates.STYLE)
               .SubstateOf(NavigationStates.MORE_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.KEYBOARD_CONTROL)
               .Permit(NavigationTriggers.DOWN, NavigationStates.FONT)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.STYLE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.STYLE.ExecuteRight);

            NavigationStates.FONT.LeftAction = PrevFont;
            NavigationStates.FONT.RightAction = NextFont;
            Navigation.Configure(NavigationStates.FONT)
               .SubstateOf(NavigationStates.MORE_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.STYLE)
               .Permit(NavigationTriggers.DOWN, NavigationStates.MENU_MORE)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.FONT.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.FONT.ExecuteRight);

            Navigation.OnUnhandledTrigger((state, trigger) =>
            {
                Console.WriteLine("OnUnhandledTrigger {0} {1}", state, trigger);
            });

            Navigation.OnTransitionCompleted(a => UpdateModelFromMavigation());
        }

        [RelayCommand]
        private void Select(NavigationState navigationState)
        {
            // Console.WriteLine("Select {0}", navigationState);
            SelectNavigationState(navigationState);
        }

        private void Activate()
        {
            Console.WriteLine("Activate");
            MainWindowViewModel.Instance?.Activate();
        }

        public bool OnKeyPressed(KeyEvent keyEvent)
        {
            //Debug.WriteLine("ListenerOnKeyPressed {0}", key);
            if (keyEvent.alt)
                return false;

            var key = keyEvent.key;

            NavigationState.Repeat = keyEvent.repeated;
            var isVerticalNavigation = key == GlobalKey.VK_UP || key == GlobalKey.VK_DOWN;

            if (NavigationState.Repeat && (!State!.AllowRepeat || isVerticalNavigation))
            {
                //Console.WriteLine("Skip {0}", keyEvent.key);
                return false;
            }

            //Console.WriteLine("Execute {0} {1} {2}", State.Name, State.AllowRepeat, keyEvent.key);

            if (key == GlobalKey.VK_LEFT)
            {
                Navigation.Fire(NavigationTriggers.LEFT);
                return true;
            }

            if (key == GlobalKey.VK_RIGHT)
            {
                Navigation.Fire(NavigationTriggers.RIGHT);
                return true;
            }

            if (key == GlobalKey.VK_UP)
            {
                Navigation.Fire(NavigationTriggers.UP);
                return true;
            }

            if (key == GlobalKey.VK_DOWN)
            {
                Navigation.Fire(NavigationTriggers.DOWN);
                return true;
            }

            return false;
        }

        void NextStyle()
        {
            Console.WriteLine("NextStyle");
            var currentStyleIndex = Array.IndexOf(Styles, NavigationStates.STYLE.SelectionLabel);
            var nextStyleIndex = (currentStyleIndex + 1) % Styles.Length;
            NavigationStates.STYLE.SelectionLabel = Styles[nextStyleIndex];
            App.SelectStyle(NavigationStates.STYLE.SelectionLabel, NavigationStates.FONT.SelectionLabel);

        }
        void PrevStyle()
        {
            Console.WriteLine("PrevStyle");
            var currentStyleIndex = Array.IndexOf(Styles, NavigationStates.STYLE.SelectionLabel);
            var prevStyleIndex = (currentStyleIndex - 1 + Styles.Length) % Styles.Length;
            NavigationStates.STYLE.SelectionLabel = Styles[prevStyleIndex];
            App.SelectStyle(NavigationStates.STYLE.SelectionLabel, NavigationStates.FONT.SelectionLabel);
        }

        string[] fontList()
        {
            var fontsFolder = Path.Combine(Entry.VersionPath, "Fonts");
            string[] fileEntries = Directory.GetFiles(fontsFolder, "*.ttf");
            List<string> fonts = [];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                var ff = Fonts.GetFontFamilies(fileEntries[i]);
                if (ff.Count > 0)
                {
                    var y = ff.First();
                    var k = y.Source.Split("#");
                    var v = k[k.Length - 1];
                    Console.WriteLine("fontCol.Families[0].Name {0}", v);
                    fonts.Add(v);
                }
            }
            return fonts.ToArray();
        }

        void NextFont()
        {
            Console.WriteLine("NextFont");
            var Fonts = fontList();
            if (Fonts.Length == 0) return;
            var currentStyleIndex = Array.IndexOf(Fonts, NavigationStates.FONT.SelectionLabel);
            var nextStyleIndex = (currentStyleIndex + 1) % Fonts.Length;
            NavigationStates.FONT.SelectionLabel = Fonts[nextStyleIndex];
            App.SelectStyle(NavigationStates.STYLE.SelectionLabel, NavigationStates.FONT.SelectionLabel);

        }
        void PrevFont()
        {
            Console.WriteLine("PrevFont");
            var Fonts = fontList();
            if (Fonts.Length == 0) return;
            var currentStyleIndex = Array.IndexOf(Fonts, NavigationStates.FONT.SelectionLabel);
            var prevStyleIndex = (currentStyleIndex - 1 + Fonts.Length) % Fonts.Length;
            NavigationStates.FONT.SelectionLabel = Fonts[prevStyleIndex];
            App.SelectStyle(NavigationStates.STYLE.SelectionLabel, NavigationStates.FONT.SelectionLabel);
        }

        void UpdateModelFromMavigation()
        {
            // Console.WriteLine("UpdateModelFromStateless {0} ", nav.State);

            State = Navigation.State;

            var newStates = new Dictionary<string, NavigationState> { };

            var info = Navigation.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                var navigationState = (NavigationState)stateInfo.UnderlyingState;
                var isInState = Navigation.IsInState(navigationState);
                navigationState.Selected = isInState;
                navigationState.Visibility = isInState ? Visibility.Visible : Visibility.Collapsed;

                newStates[navigationState.Name] = navigationState;
            }

            States = newStates;
        }

        public static void SelectNavigationState(NavigationState navigationState)
        {
            if (Instance == null) return;

            if (!Instance.Navigation.IsInState(navigationState))
            {
                DirectNavigationStateTarget = navigationState;
                Instance.Navigation.Fire(NavigationTriggers.DIRECT);
            }
        }
    }
}