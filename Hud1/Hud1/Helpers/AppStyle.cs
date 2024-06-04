using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Hud1.Helpers;
internal class AppStyle
{
    internal static void SelectStyle(string style, string fontFriendlyName)
    {
        Debug.Print($"AppStyle SelectStyle {style} {fontFriendlyName}");

#if HOT
        // dont touch Application.Current.Resources.MergedDictionaries otherwise Hot Reload wount work 
        return;
#endif

        // font iterator
        var fontsFolder = Path.Combine(Path.Combine(Setup.VersionPath, "Fonts"), ".");
        var fonts = HudFonts.GetFonts();
        fonts.ForEach((HudFont font) =>
        {
            if (font.Name.Equals(fontFriendlyName))
            {
                // Debug.Print($"\t'{font.Name}' Weight: {font.Typeface.Weight} Style: {font.Typeface.Style} Stretch: {font.Typeface.Stretch}");
                ReplaceResource(0, new ResourceDictionary
                {
                    { "FontFamily", new FontFamily(new Uri(fontsFolder, UriKind.Absolute), $"./#{font.Name}") },
                    { "FontStyle", font.Typeface.Style},
                    { "FontWeight",font.Typeface.Weight },
                    { "FontStretch", font.Typeface.Stretch },
                });
            }
        });

        ReplaceResource(1, new ResourceDictionary
        {
            Source = new Uri("/Game Direct;Component/Styles/" + style + ".xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(2, new ResourceDictionary
        {
            Source = new Uri("/Game Direct;Component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(3, new ResourceDictionary
        {
            Source = new Uri("/Game Direct;Component/Styles/Labels.xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(4, new ResourceDictionary
        {
            Source = new Uri("/Game Direct;Component/Styles/Buttons.xaml", UriKind.RelativeOrAbsolute)
        });

        ReplaceResource(5, new ResourceDictionary
        {
            Source = new Uri("/Game Direct;Component/Styles/ScrollViewer.xaml", UriKind.RelativeOrAbsolute)
        });
    }

    private static void ReplaceResource(int index, ResourceDictionary dictionary)
    {
        Application.Current.Resources.MergedDictionaries.Insert(index, dictionary);
        Application.Current.Resources.MergedDictionaries.RemoveAt(index + 1);
    }
}
