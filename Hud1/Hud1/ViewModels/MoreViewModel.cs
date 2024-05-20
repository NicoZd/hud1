using Hud1.Models;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public class MoreViewModel
    {
        private readonly string[] Styles = ["Green", "Red"];

        public static readonly MoreViewModel Instance = new();

        private MoreViewModel()
        {
        }

        public void BuildNavigation()
        {
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
        }

        private void Activate()
        {
            Console.WriteLine("Activate");
            MainWindowViewModel.Instance?.Activate();
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

    }
}
