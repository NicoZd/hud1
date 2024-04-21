using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Hud1.Controls
{
    public partial class CustomControl2Model : ObservableObject
    {
        public Boolean _selected = false;
        public Boolean Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged(); OnPropertyChanged("Background"); }
        }

        [ObservableProperty]
        public String label = "";

        public Brush Background
        {
            get
            {
                //Debug.Print("Background Get {0}", Selected);

                var colHig = (Color)ColorConverter.ConvertFromString("#ff55ee22");
                var colLow = (Color)ColorConverter.ConvertFromString("#ee009900");
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0, 0);
                myLinearGradientBrush.EndPoint = new Point(1, 0);
                myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 0.0));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 0.035));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(colLow, 0.035));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(colLow, 0.965));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 0.965));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 1.0));

                return Selected ?
                    myLinearGradientBrush :
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aa007700"));

            }
            set { }
        }
    }
}
