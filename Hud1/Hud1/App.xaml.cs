using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Windows.Storage;

namespace Hud1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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

        public static void SelectStyle(String style, String font)
        {
            NavigationStates.FONT.SelectionLabel = font;

            var exeFolder = Path.GetDirectoryName(Process.GetCurrentProcess()!.MainModule!.FileName);

            var fontsFolder = Path.Combine(exeFolder!, "Fonts");
            var fontFolder = "";

            string[] fileEntries = Directory.GetFiles(fontsFolder, "*.ttf");

            Console.WriteLine("files {0}", fileEntries.Length);

            List<string> fonts = [];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fileEntries[i]);

                Console.WriteLine("fontCol.Families[0].Name {0}", fontCol.Families[0].Name);

                if (fontCol.Families[0].Name.Equals(font))
                {
                    fontFolder = Path.Combine(exeFolder!, fileEntries[i]);
                }
                fontCol.Dispose();
            }

            Console.WriteLine("Found {0}", fontFolder);
            var ff = new FontFamily(new Uri(fontFolder, UriKind.Absolute), "./#" + font);


            var x = new ResourceDictionary();
            x.Add("FontFamily", ff);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(x);

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("Themes/" + style + ".xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("Themes/Standard.xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("Themes/Buttons.xaml", UriKind.RelativeOrAbsolute)
            });
        }

    }
}
