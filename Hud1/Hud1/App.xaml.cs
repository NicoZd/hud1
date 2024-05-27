using Hud1.Helpers;
using Hud1.Models;
using Hud1.Start;
using Hud1.Views;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hud1;

public partial class App : Application
{
    public App()
    {
        Debug.Print("App {0} {1} {2}", Entry.Millis(), ShutdownMode, MainWindow);
        InitializeComponent();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
        EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
    }

    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
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

    public static void SelectStyle(string style, string font)
    {
        //return;

        // for testing
        if (Application.Current == null) return;

        NavigationStates.FONT.SelectionLabel = font;

        var fontFile = "";

        var fontsFolder = Path.Combine(Setup.VersionPath, "Fonts");
        if (Directory.Exists(fontsFolder))
        {
            var fileEntries = Directory.GetFiles(fontsFolder, "*.*").Where(s => s.ToLower().EndsWith(".ttf") || s.ToLower().EndsWith(".otf")).ToArray();
            for (var i = 0; i < fileEntries.Length; i++)
            {
                var ff = Fonts.GetFontFamilies(fileEntries[i]);
                if (ff.Count > 0)
                {
                    var y = ff.First();
                    var k = y.Source.Split("#");
                    var v = k[^1];
                    Console.WriteLine("fontCol.Families[0].Name {0}", v);
                    if (v.Equals(font))
                    {
                        fontFile = fileEntries[i];
                        break;
                    }
                }
            }
        }


        foreach (var sp in ScrollPanel.Instances)
            sp.SaveScrollPosition();

        if (fontFile != "")
        {

            ReplaceResource(0, new ResourceDictionary
        {
            { "FontFamily", new FontFamily(new Uri(fontFile, UriKind.Absolute), "./#" + font) }
        });
        }

        ReplaceResource(1, new ResourceDictionary
        {
            Source = new Uri("Styles/" + style + ".xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(2, new ResourceDictionary
        {
            Source = new Uri("Styles/Standard.xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(3, new ResourceDictionary
        {
            Source = new Uri("Styles/Buttons.xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(4, new ResourceDictionary
        {
            Source = new Uri("Styles/ScrollViewer.xaml", UriKind.RelativeOrAbsolute)
        });

        foreach (var sp in ScrollPanel.Instances)
            sp.RestoreScrollPosition();

    }

}
