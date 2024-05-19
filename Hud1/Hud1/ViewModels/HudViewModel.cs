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
            Navigation = new(NavigationStates.MENU_NIGHTVISION);

            BuildNavigation();

            //string graph = UmlDotGraph.Format(Navigation.GetInfo());
            //Console.WriteLine(graph);

            MacrosViewModel.Navigation = Navigation;

            UpdateModelFromMavigation();
        }

        private void BuildNavigation()
        {
            Navigation.Configure(NavigationStates.ALL)
                .PermitDynamic(NavigationTriggers.DIRECT, () => { return DirectNavigationStateTarget!; });

            Navigation.Configure(NavigationStates.NIGHTVISION_VISIBLE).SubstateOf(NavigationStates.ALL);
            Navigation.Configure(NavigationStates.MACRO_VISIBLE).SubstateOf(NavigationStates.ALL);
            Navigation.Configure(NavigationStates.CROSSHAIR_VISIBLE).SubstateOf(NavigationStates.ALL);
            Navigation.Configure(NavigationStates.MORE_VISIBLE).SubstateOf(NavigationStates.ALL);

            // MENU
            Navigation.Configure(NavigationStates.MENU_NIGHTVISION)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_CROSSHAIR);

            Navigation.Configure(NavigationStates.MENU_CROSSHAIR)
                .SubstateOf(NavigationStates.CROSSHAIR_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_NIGHTVISION)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MACRO);

            Navigation.Configure(NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_CROSSHAIR)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MORE);

            Navigation.Configure(NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_NIGHTVISION);

            // NIGHTVISION

            NavigationStates.NIGHTVISION_ENABLED.LeftAction = GammaViewModel.SelectPrevGama;
            NavigationStates.NIGHTVISION_ENABLED.RightAction = GammaViewModel.SelectNextGama;
            Navigation.Configure(NavigationStates.NIGHTVISION_ENABLED)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.NIGHTVISION_ENABLED.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.NIGHTVISION_ENABLED.ExecuteRight);

            NavigationStates.GAMMA.LeftAction = GammaViewModel.SelectPrevGama;
            NavigationStates.GAMMA.RightAction = GammaViewModel.SelectNextGama;
            Navigation.Configure(NavigationStates.GAMMA)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.GAMMA.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.GAMMA.ExecuteRight);

            MakeNav(NavigationStates.MENU_NIGHTVISION, NavigationStates.NIGHTVISION_VISIBLE,
                [NavigationStates.NIGHTVISION_ENABLED, NavigationStates.GAMMA]);

            // MACROS
            Navigation.Configure(NavigationStates.MACROS)
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
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.MACROS_FOLDER.ExecuteRight);

            MakeNav(NavigationStates.MENU_MACRO, NavigationStates.MACRO_VISIBLE,
                [NavigationStates.MACROS, NavigationStates.MACROS_FOLDER]);

            // MORE

            NavigationStates.EXIT.RightAction = Application.Current.Shutdown;
            Navigation.Configure(NavigationStates.EXIT)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.EXIT.ExecuteRight);

            NavigationStates.ACTIVATE.RightAction = Activate;
            Navigation.Configure(NavigationStates.ACTIVATE)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.ACTIVATE.ExecuteRight);

            Navigation.Configure(NavigationStates.HUD_POSITION)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.HUD_POSITION.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.HUD_POSITION.ExecuteRight);

            Navigation.Configure(NavigationStates.KEYBOARD_CONTROL)
                .InternalTransition(NavigationTriggers.LEFT, NavigationStates.KEYBOARD_CONTROL.ExecuteLeft)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.KEYBOARD_CONTROL.ExecuteRight);

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


            MakeNav(NavigationStates.MENU_MORE, NavigationStates.MORE_VISIBLE,
                [NavigationStates.EXIT, NavigationStates.ACTIVATE, NavigationStates.HUD_POSITION,
                NavigationStates.KEYBOARD_CONTROL, NavigationStates.STYLE, NavigationStates.FONT]);

            Navigation.OnUnhandledTrigger((state, trigger) =>
            {
                Console.WriteLine("OnUnhandledTrigger {0} {1}", state, trigger);
            });

            Navigation.OnTransitionCompleted(a => UpdateModelFromMavigation());
        }

        private void MakeNav(NavigationState menu, NavigationState visible, NavigationState[] list)
        {
            if (list.Length < 2)
                throw new Exception("List Length mist be at least 2.");

            var first = list[0];
            var last = list[list.Length - 1];

            Navigation.Configure(menu)
                .SubstateOf(visible)
                .Permit(NavigationTriggers.UP, last)
                .Permit(NavigationTriggers.DOWN, first);

            for (var i = 0; i < list.Length; i++)
            {
                var item = list[i];
                Navigation.Configure(item).SubstateOf(visible);

                if (item == NavigationStates.MACROS)
                    continue;

                if (item == first)
                {
                    Navigation.Configure(item)
                        .Permit(NavigationTriggers.UP, menu)
                        .Permit(NavigationTriggers.DOWN, list[i + 1]);
                }
                else if (item == last)
                {
                    Navigation.Configure(item)
                        .Permit(NavigationTriggers.UP, list[i - 1])
                        .Permit(NavigationTriggers.DOWN, menu);
                }
                else
                {
                    Navigation.Configure(item)
                        .Permit(NavigationTriggers.UP, list[i - 1])
                        .Permit(NavigationTriggers.DOWN, list[i + 1]);
                }
            }
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
            var fontsFolder = Path.Combine(Startup.VersionPath, "Fonts");
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