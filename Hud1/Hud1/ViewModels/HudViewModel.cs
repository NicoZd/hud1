using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.Models;
using Stateless.Reflection;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public partial class HudViewModel : ObservableObject
    {
        private readonly string[] Styles = ["Green", "Red"];

        public readonly static HudViewModel Instance = new();

        [ObservableProperty]
        public NavigationState? state;

        [ObservableProperty]
        public Dictionary<string, NavigationState> states = new Dictionary<string, NavigationState> { };

        private HudViewModel()
        {
        }

        public void BuildNavigation()
        {
            Debug.Print("HudViewModel BuildNavigation");
            var Navigation = NavigationViewModel.Instance.Navigation;

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


            NavigationViewModel.MakeNav(NavigationStates.MENU_MORE, NavigationStates.MORE_VISIBLE,
                [NavigationStates.EXIT, NavigationStates.ACTIVATE, NavigationStates.HUD_POSITION,
                NavigationStates.KEYBOARD_CONTROL, NavigationStates.STYLE, NavigationStates.FONT]);

            Navigation.OnUnhandledTrigger((state, trigger) =>
            {
                Console.WriteLine("OnUnhandledTrigger {0} {1}", state, trigger);
            });

            Navigation.OnTransitionCompleted(a => UpdateModelFromMavigation());

            UpdateModelFromMavigation();
        }

        [RelayCommand]
        private void Select(NavigationState navigationState)
        {
            // Console.WriteLine("Select {0}", navigationState);
            NavigationViewModel.SelectNavigationState(navigationState);
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
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.LEFT);
                return true;
            }

            if (key == GlobalKey.VK_RIGHT)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.RIGHT);
                return true;
            }

            if (key == GlobalKey.VK_UP)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.UP);
                return true;
            }

            if (key == GlobalKey.VK_DOWN)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.DOWN);
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

            var Navigation = NavigationViewModel.Instance.Navigation;

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
    }
}