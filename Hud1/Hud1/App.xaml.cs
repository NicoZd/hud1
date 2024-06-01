using Hud1.Views;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Hud1;

public partial class App : Application
{
    public App()
    {
        Debug.Print("App {0} {1} {2}", Entry.Millis(), ShutdownMode, MainWindow);
        // PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.All;
        InitializeComponent();
    }

    public static void ReplaceResource(int index, ResourceDictionary dictionary)
    {
        Application.Current.Resources.MergedDictionaries.Insert(index, dictionary);
        Application.Current.Resources.MergedDictionaries.RemoveAt(index + 1);
    }

    public static void SelectStyle(string style, string font)
    {
#if HOT
        // dont touch Application.Current.Resources.MergedDictionaries otherwise Hot Reload wount work 
        return;
#endif

        // for testing
        if (Application.Current == null) return;

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
