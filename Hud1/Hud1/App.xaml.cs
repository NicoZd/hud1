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

            //var dir = "C:\\Users\\nicoz\\AppData\\Local\\Hud1";
            //if (!Directory.Exists(dir))
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //var r = new Random();
            //using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, "Write" + r.NextDouble() + "Lines.txt")))
            //{
            //    outputFile.WriteLine("line");
            //}

            //TestIO();


            //SelectStyle("Green", "Source Code Pro");

            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
        }

        private async void TestIO()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Debug.Print("storageFolder {0}", storageFolder);


            StorageFile file = await storageFolder.CreateFileAsync("test.txt",
                    CreationCollisionOption.OpenIfExists);

            await FileIO.WriteTextAsync(file, "Example of writing a string\r\n");

            // Append a list of strings, one per line, to the file
            var listOfStrings = new List<string> { "line1", "line2", "line3" };
            await FileIO.AppendLinesAsync(file, listOfStrings);

            Process.Start("explorer.exe", storageFolder.Path);

            StorageFile file2 = await storageFolder.CreateFileAsync("test2.txt",
                   CreationCollisionOption.OpenIfExists);

            using (StreamWriter outputFile = new StreamWriter(file2.Path))
            {
                outputFile.WriteLine("line");
            }

            var dir = Path.Combine(storageFolder.Path, "manual");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var r = new Random();
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, "Write" + r.NextDouble() + "Lines.txt")))
            {
                outputFile.WriteLine("line");
            }
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

            Debug.Print("files {0}", fileEntries.Length);

            List<string> fonts = [];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fileEntries[i]);

                Debug.Print("fontCol.Families[0].Name {0}", fontCol.Families[0].Name);

                if (fontCol.Families[0].Name.Equals(font))
                {
                    fontFolder = Path.Combine(exeFolder!, fileEntries[i]);
                }
                fontCol.Dispose();
            }

            Debug.Print("Found {0}", fontFolder);
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
