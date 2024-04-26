using Hud1.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Hud1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((a, b) =>
            {
                SelectStyle(NavigationStates.STYLE.SelectionLabel);
                //SelectStyle2(NavigationStates.STYLE.SelectionLabel);
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        public static void SelectStyle(String name)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("Themes/" + name + ".xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("Themes/Standard.xaml", UriKind.RelativeOrAbsolute)
            });
        }

        public static void SelectStyleOld(String name)
        {
            Debug.Print("SelectStyle {0}", name);
            var Resources = Application.Current.Resources;
            switch (name)
            {
                case "Green":
                    Resources["ColorSuperBright"] = (Color)ColorConverter.ConvertFromString("#bbffbb");
                    Resources["ColorBright"] = (Color)ColorConverter.ConvertFromString("#66ff66");
                    Resources["ColorBackgroundDark"] = (Color)ColorConverter.ConvertFromString("#f5008800");
                    Resources["ColorBackgroundDarkMed"] = (Color)ColorConverter.ConvertFromString("#dd005500");
                    Resources["ColorBackgroundDarkTrans"] = (Color)ColorConverter.ConvertFromString("#11005500");
                    Resources["ColorInfo"] = (Color)ColorConverter.ConvertFromString("#bbffbb");
                    break;
                case "Dark":
                    Resources["ColorSuperBright"] = (Color)ColorConverter.ConvertFromString("#99ccff");
                    Resources["ColorBright"] = (Color)ColorConverter.ConvertFromString("#3399cc");
                    break;
                case "Red":
                    Resources["ColorSuperBright"] = (Color)ColorConverter.ConvertFromString("#b55050");
                    Resources["ColorBright"] = (Color)ColorConverter.ConvertFromString("#9f392e");
                    Resources["ColorMed"] = (Color)ColorConverter.ConvertFromString("#ff45191c");
                    Resources["ColorBackgroundDark"] = (Color)ColorConverter.ConvertFromString("#ff0f060a");
                    Resources["ColorBackgroundDarkMed"] = (Color)ColorConverter.ConvertFromString("#ee1f0f15");
                    Resources["ColorBackgroundDarkTrans"] = (Color)ColorConverter.ConvertFromString("#9905050d");
                    Resources["ColorInfo"] = (Color)ColorConverter.ConvertFromString("#51e8fe");
                    break;
            }

            // update brushes
            Resources["SeparatorColor"] = new SolidColorBrush((Color)Resources["ColorBackgroundDark"]);
            Resources["TitleColor"] = new SolidColorBrush((Color)Resources["ColorSuperBright"]);
            Resources["LabelColor"] = new SolidColorBrush((Color)Resources["ColorSuperBright"]);
            Resources["InfoColor"] = new SolidColorBrush((Color)Resources["ColorInfo"]);

            //var mainWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            //Debug.Print("mainWindow {0}", mainWindow);
            //mainWindow?.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);

            //Application.Current.Dispatcher?.Invoke(DispatcherPriority.Render, EmptyDelegate);
            //            if (mainWindow != null)
            //                EnumVisual(mainWindow!);

            //mainWindow.Visibility = Visibility.Collapsed;
            //mainWindow.Visibility = Visibility.Visible;

        }


    }

}
