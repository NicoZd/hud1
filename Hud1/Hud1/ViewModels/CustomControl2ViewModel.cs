using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public partial class CustomControl2ViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("Background", "Foreground", "Border")]
        public Boolean selected = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor("Background", "Foreground", "Border")]
        public Boolean pressed = false;

        [ObservableProperty]
        public String label = "";

        public Brush Background
        {
            get
            {
                if (Selected && !Pressed)
                {
                    var colHig = (Color)Application.Current.Resources["ColorBackgroundDark"];
                    var colLow = (Color)Application.Current.Resources["ColorBackgroundDarkMed"];
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0, 0);
                    brush.EndPoint = new Point(1, 0);
                    brush.GradientStops.Add(new GradientStop(colHig, 0.0));
                    brush.GradientStops.Add(new GradientStop(colLow, 0.5));
                    brush.GradientStops.Add(new GradientStop(colHig, 1));
                    return brush;
                }
                else if (Pressed)
                {
                    var colHig = (Color)ColorConverter.ConvertFromString("#9900ff00");
                    var colLow = (Color)ColorConverter.ConvertFromString("#6600ff00");
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0, 0);
                    brush.EndPoint = new Point(1, 0);
                    brush.GradientStops.Add(new GradientStop(colHig, 0.0));
                    brush.GradientStops.Add(new GradientStop(colLow, 0.5));
                    brush.GradientStops.Add(new GradientStop(colHig, 1));
                    return brush;
                }
                else
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorBackgroundDark"]);
                }
            }
        }

        public Brush Foreground
        {
            get
            {
                if (Selected && !Pressed)
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorInfo"]);
                }
                else if (Pressed)
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorInfo"]);
                }
                else
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorBright"]);
                }
            }
        }

        public Brush Border
        {
            get
            {
                if (Selected && !Pressed)
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorInfo"]);
                }
                else if (Pressed)
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorInfo"]);
                }
                else
                {
                    return new SolidColorBrush((Color)Application.Current.Resources["ColorBright"]);
                }
            }
        }
    }
}
