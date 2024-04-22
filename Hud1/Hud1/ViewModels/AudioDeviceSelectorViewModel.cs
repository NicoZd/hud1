using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public partial class AudioDeviceSelectorViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("Background")]
        public Boolean selected = false;


        [ObservableProperty]
        public String label = "";

        public Brush Background
        {
            get
            {
                if (Selected)
                {
                    var colHig = (Color)ColorConverter.ConvertFromString("#ff55ee22");
                    var colLow = (Color)ColorConverter.ConvertFromString("#ee009900");
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0, 0);
                    brush.EndPoint = new Point(1, 0);
                    brush.GradientStops.Add(new GradientStop(colHig, 0.0));
                    brush.GradientStops.Add(new GradientStop(colHig, 0.035));
                    brush.GradientStops.Add(new GradientStop(colLow, 0.035));
                    brush.GradientStops.Add(new GradientStop(colLow, 0.965));
                    brush.GradientStops.Add(new GradientStop(colHig, 0.965));
                    brush.GradientStops.Add(new GradientStop(colHig, 1.0));
                    return brush;
                }
                else
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aa007700"));
                }
            }
        }
    }
}
