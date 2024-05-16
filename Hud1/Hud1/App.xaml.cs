using Hud1.Helpers;
using Hud1.Helpers.CustomSplashScreen;
using Hud1.Models;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hud1
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
        }

        static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseService.IgnoreNextEvent)
            {
                e.Handled = true;
            }
        }

        public static void ReplaceResource(int index, ResourceDictionary dictionary)
        {
            Application.Current.Resources.MergedDictionaries.Insert(index, dictionary);
            Application.Current.Resources.MergedDictionaries.RemoveAt(index + 1);
        }

        public static void SelectStyle(String style, String font)
        {
            NavigationStates.FONT.SelectionLabel = font;

            var fontFile = "";

            var fontsFolder = Path.Combine(Entry.VersionPath, "Fonts");
            string[] fileEntries = Directory.GetFiles(fontsFolder, "*.ttf");
            for (int i = 0; i < fileEntries.Length; i++)
            {
                var ff = Fonts.GetFontFamilies(fileEntries[i]);
                if (ff.Count > 0)
                {
                    var y = ff.First();
                    var k = y.Source.Split("#");
                    var v = k[k.Length - 1];
                    Console.WriteLine("fontCol.Families[0].Name {0}", v);
                    if (v.Equals(font))
                    {
                        fontFile = fileEntries[i];
                        break;
                    }
                }
            }

            ReplaceResource(0, new ResourceDictionary
            {
                { "FontFamily", new FontFamily(new Uri(fontFile, UriKind.Absolute), "./#" + font) }
            });

            ReplaceResource(1, new ResourceDictionary
            {
                Source = new Uri("Themes/" + style + ".xaml", UriKind.RelativeOrAbsolute)
            });

            ReplaceResource(2, new ResourceDictionary
            {
                Source = new Uri("Themes/Standard.xaml", UriKind.RelativeOrAbsolute)
            });

            ReplaceResource(3, new ResourceDictionary
            {
                Source = new Uri("Themes/Buttons.xaml", UriKind.RelativeOrAbsolute)
            });

            ReplaceResource(4, new ResourceDictionary
            {
                Source = new Uri("Themes/ScrollViewer.xaml", UriKind.RelativeOrAbsolute)
            });
        }

    }
}
