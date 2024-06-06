using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Hud1.Helpers;


internal class HudFont
{
    internal string Name;
    internal Typeface Typeface;

    internal HudFont(string name, Typeface typeface)
    {
        Name = name;
        Typeface = typeface;
    }
}

internal class HudFonts
{
    internal static List<HudFont> GetFonts()
    {
        var result = new List<HudFont>();

        var fontsFolder = Path.Combine(Setup.UserDataPath, "Fonts");
        var fileEntries = Directory.GetFiles(fontsFolder, "*.*").Where(s => s.ToLower().EndsWith(".ttf") || s.ToLower().EndsWith(".otf")).ToArray();
        HashSet<string> perms = [];
        for (var i = 0; i < fileEntries.Length; i++)
        {
            //Debug.Print($"========= fileEntries[i] {fileEntries[i]}");

            var fontFamilies = Fonts.GetFontFamilies(fileEntries[i]);
            foreach (var fontFamily in fontFamilies)
            {
                var fontFamilyName = fontFamily.Source.Split("#")[1];
                //Console.WriteLine($"FontFamily: {fontFamilyName}");
                foreach (var typeface in fontFamily.GetTypefaces())
                {
                    var perm = $"FontFamily: '{fontFamilyName}' Weight: {typeface.Weight} IsBoldSimulated:{typeface.IsBoldSimulated} Stretch:{typeface.Stretch} Style:{typeface.Style} IsObliqueSimulated:{typeface.IsObliqueSimulated}";
                    //Console.WriteLine(perm);

                    if (typeface.IsBoldSimulated)
                        continue;

                    if (typeface.IsObliqueSimulated)
                        continue;

                    List<string> elements = [];
                    elements.Add(fontFamilyName);

                    if (typeface.Weight != FontWeights.Normal)
                        elements.Add(typeface.Weight.ToString());

                    if (typeface.Style != FontStyles.Normal)
                        elements.Add(typeface.Style.ToString());

                    if (typeface.Stretch != FontStretches.Normal)
                        elements.Add(typeface.Stretch.ToString());

                    var friendlyName = string.Join(" ", elements);
                    if (perms.Contains(friendlyName))
                    {
                        continue;
                    }
                    perms.Add(friendlyName);
                    result.Add(new HudFont(friendlyName, typeface));
                }
            }
        }

        return [.. result.OrderBy(i => i.Name)];
    }
}
