using Hud1.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfScreenHelper;

namespace Hud1.ViewModels;

public class CrosshairViewModel
{
    public static readonly CrosshairViewModel Instance = new();

    public static readonly List<string> Forms = ["a", "b", "c"];

    private CrosshairViewModel() { }

    public void BuildNavigation()
    {
        var Navigation = NavigationViewModel.Instance.Navigation;

        Navigation.Configure(NavigationStates.CROSSHAIR_ENABLED);
        NavigationStates.CROSSHAIR_ENABLED.LeftAction = NavigationStates.CROSSHAIR_ENABLED.BooleanLeft;
        NavigationStates.CROSSHAIR_ENABLED.RightAction = NavigationStates.CROSSHAIR_ENABLED.BooleanRight;

        Navigation.Configure(NavigationStates.CROSSHAIR_FORM);
        NavigationStates.CROSSHAIR_FORM.SelectionLabel = "circle";
        NavigationStates.CROSSHAIR_FORM.Options = ["dot", "circle", "cross", "diagonal", "3 dots"];
        NavigationStates.CROSSHAIR_FORM.LeftAction = NavigationStates.CROSSHAIR_FORM.OptionLeft;
        NavigationStates.CROSSHAIR_FORM.RightAction = NavigationStates.CROSSHAIR_FORM.OptionRight;

        Navigation.Configure(NavigationStates.CROSSHAIR_COLOR);
        NavigationStates.CROSSHAIR_COLOR.SelectionLabel = "red";
        NavigationStates.CROSSHAIR_COLOR.Options = ["red", "green", "blue", "white"];
        NavigationStates.CROSSHAIR_COLOR.LeftAction = NavigationStates.CROSSHAIR_COLOR.OptionLeft;
        NavigationStates.CROSSHAIR_COLOR.RightAction = NavigationStates.CROSSHAIR_COLOR.OptionRight;

        Navigation.Configure(NavigationStates.CROSSHAIR_SIZE);
        NavigationStates.CROSSHAIR_SIZE.SelectionLabel = "2";
        NavigationStates.CROSSHAIR_SIZE.Options = ["1", "2", "3"];
        NavigationStates.CROSSHAIR_SIZE.LeftAction = NavigationStates.CROSSHAIR_SIZE.OptionLeft;
        NavigationStates.CROSSHAIR_SIZE.RightAction = NavigationStates.CROSSHAIR_SIZE.OptionRight;

        Navigation.Configure(NavigationStates.CROSSHAIR_OUTLINE);
        NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean = true;
        NavigationStates.CROSSHAIR_OUTLINE.LeftAction = NavigationStates.CROSSHAIR_OUTLINE.BooleanLeft;
        NavigationStates.CROSSHAIR_OUTLINE.RightAction = NavigationStates.CROSSHAIR_OUTLINE.BooleanRight;

        NavigationViewModel.MakeNav(NavigationStates.MENU_CROSSHAIR, NavigationStates.CROSSHAIR_VISIBLE,
            [
            NavigationStates.CROSSHAIR_ENABLED,
            NavigationStates.CROSSHAIR_FORM,
            NavigationStates.CROSSHAIR_COLOR,
            NavigationStates.CROSSHAIR_SIZE,
            NavigationStates.CROSSHAIR_OUTLINE,
            ]);
    }

    public void Redraw(Grid grid)
    {
        Debug.Print("Enabled {0}", NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean);
        Debug.Print("Form {0}", NavigationStates.CROSSHAIR_FORM.SelectionLabel);
        Debug.Print("Size {0}", NavigationStates.CROSSHAIR_SIZE.SelectionLabel);
        Debug.Print("Color {0}", NavigationStates.CROSSHAIR_COLOR.SelectionLabel);
        Debug.Print("Outline {0}", NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean);

        var colors = new Dictionary<string, Brush>
        {
            { "red", Brushes.Red },
            { "green", Brushes.Green },
            { "blue", Brushes.Blue },
            { "white", Brushes.White }
        };

        var scale = Double.Parse(NavigationStates.CROSSHAIR_SIZE.SelectionLabel);
        var color = colors[NavigationStates.CROSSHAIR_COLOR.SelectionLabel];

        var renderFunctions = new Dictionary<string, Func<double, Brush, bool, GeometryDrawing>>
        {
            { "dot", CrosshairForms.RenderDot },
            { "circle", CrosshairForms.RenderCircle },
            { "cross", CrosshairForms.RenderDot },
            { "diagonal", CrosshairForms.RenderDot },
            { "3 dots", CrosshairForms.RenderDot },
        };


        if (!renderFunctions.ContainsKey(NavigationStates.CROSSHAIR_FORM.SelectionLabel))
        {
            Debug.Print("Form is null {0}", NavigationStates.CROSSHAIR_FORM.SelectionLabel);
            return;
        }

        var formFunction = renderFunctions[NavigationStates.CROSSHAIR_FORM.SelectionLabel];

        var geometryDrawing = formFunction(scale, color, NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean);
        var dpiScale = 1 / Screen.AllScreens.ElementAt(0).ScaleFactor;

        DrawingImage geometryImage = new(geometryDrawing);
        geometryImage.Freeze();

        Image image = new()
        {
            Source = geometryImage,
            Stretch = Stretch.None,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            RenderTransform = new ScaleTransform(dpiScale, dpiScale, geometryImage.Width / 2.0, geometryImage.Height / 2.0)
        };

        grid.Children.Clear();
        grid.Children.Add(image);
    }
}
