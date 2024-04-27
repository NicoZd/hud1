using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Hud1.Models
{
    public partial class NavigationState : ObservableObject
    {
        [ObservableProperty]
        public bool selectRight;

        [ObservableProperty]
        public bool selectLeft;

        [ObservableProperty]
        [NotifyPropertyChangedFor("SelectedBorder", "SelectedBackground", "SelectedBlurOpacity")]
        private bool selected;

        public Visibility Visibility { get; set; }

        public string Name { get; set; }
        public string Label { get; set; }

        public string Hint { get; set; }

        [ObservableProperty]
        public string selectionLabel;

        public Action? LeftAction { get; set; }

        public Action? RightAction { get; set; }

        public NavigationState([CallerMemberName] string Name = "")
        {
            this.Name = Name;

            Label = "";
            SelectionLabel = "";
            Selected = false;
            SelectRight = false;
            Visibility = Visibility.Collapsed;
        }

        public async void ExecuteLeft()
        {
            SelectLeft = true;
            await Task.Delay(10);
            if (LeftAction != null)
                try { LeftAction(); } catch (Exception ex) { Debug.Print("ExecuteLeft {0}", ex); }
            SelectLeft = false;
        }

        public async void ExecuteRight()
        {
            SelectRight = true;
            await Task.Delay(10);
            if (RightAction != null)
                try { RightAction(); } catch (Exception ex) { Debug.Print("ExecuteRight {0}", ex); }
            SelectRight = false;
        }

        public Brush SelectedBorder
        {
            get
            {
                if (Selected)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff00ff00"));
                }
                else
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8800ff00"));
                }
            }
        }

        public float SelectedBlurOpacity
        {
            get
            {
                if (Selected)
                {
                    return 0.2f;
                }
                else
                {
                    return 0;
                }
            }
        }

        public Brush SelectedBackground
        {
            get
            {
                if (Selected)
                {
                    var colHig = (Color)ColorConverter.ConvertFromString("#88009900");
                    var colLow = (Color)ColorConverter.ConvertFromString("#55009900");
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0, 0);
                    brush.EndPoint = new Point(1, 0);
                    //brush.GradientStops.Add(new GradientStop(colHig, 0.0));
                    //brush.GradientStops.Add(new GradientStop(colHig, 0.035));
                    brush.GradientStops.Add(new GradientStop(colHig, 0.0));
                    brush.GradientStops.Add(new GradientStop(colLow, 0.5));
                    brush.GradientStops.Add(new GradientStop(colHig, 1));
                    //brush.GradientStops.Add(new GradientStop(colHig, 0.965));
                    //brush.GradientStops.Add(new GradientStop(colHig, 1.0));
                    return brush;
                }
                else
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aa007700"));
                }
            }
        }

        public override string? ToString()
        {
            return Name;
        }
    }
}
