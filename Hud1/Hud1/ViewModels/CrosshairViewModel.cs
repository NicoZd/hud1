using Hud1.Helpers.ScreenHelper;
using Hud1.Models;
using Hud1.Start;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hud1.ViewModels;

public class CrosshairViewModel
{
    public static readonly CrosshairViewModel Instance = new();
    private Dictionary<string, Func<int, Brush, bool, Drawing>> FormRenderFunctions = [];
    private readonly Dictionary<string, Brush> ColorOptions = [];

    private CrosshairViewModel() { }

    public void BuildNavigation()
    {
        var Navigation = NavigationViewModel.Instance.Navigation;

        NavigationStates.CROSSHAIR_ENABLED.LeftAction = NavigationStates.CROSSHAIR_ENABLED.BooleanLeft;
        NavigationStates.CROSSHAIR_ENABLED.RightAction = NavigationStates.CROSSHAIR_ENABLED.BooleanRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_ENABLED)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_ENABLED.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_ENABLED.ExecuteRight);

        NavigationStates.CROSSHAIR_FORM.SelectionLabel = "Cross";
        NavigationStates.CROSSHAIR_FORM.Options = [new Option("Dot"), new Option("Ring"), new Option("Cross"), new Option("Diagonal"), new Option("3 Dots")];
        NavigationStates.CROSSHAIR_FORM.SelectOption();
        NavigationStates.CROSSHAIR_FORM.LeftAction = NavigationStates.CROSSHAIR_FORM.OptionLeft;
        NavigationStates.CROSSHAIR_FORM.RightAction = NavigationStates.CROSSHAIR_FORM.OptionRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_FORM)
          .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_FORM.ExecuteLeft)
          .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_FORM.ExecuteRight);
        FormRenderFunctions = new Dictionary<string, Func<int, Brush, bool, Drawing>>
        {
            { "Dot", CrosshairForms.RenderDot },
            { "Ring", CrosshairForms.RenderRing },
            { "Cross", CrosshairForms.RenderCross },
            { "Diagonal", CrosshairForms.RenderDiagonal },
            { "3 Dots", CrosshairForms.ThreeDots },
        };

        NavigationStates.CROSSHAIR_COLOR.SelectionLabel = "#FFFFFF";
        NavigationStates.CROSSHAIR_COLOR.Spacing = 0;
        NavigationStates.CROSSHAIR_COLOR.Options = [

            new Option("#00C000"),
            new Option("#00FF00"),
            new Option("#B2FF00"),
            new Option("#FFFF00"),
            new Option("#FFAC00"),

            new Option("#FF3100"),
            new Option("#D10133"),
            new Option("#FF00FF"),
            new Option("#C000FF"),
            new Option("#0000FF"),

            new Option("#0080FF"),
            new Option("#00FFFF"),
            new Option("#FFFFFF"),
            ];
        NavigationStates.CROSSHAIR_COLOR.SelectOption();
        NavigationStates.CROSSHAIR_COLOR.LeftAction = NavigationStates.CROSSHAIR_COLOR.OptionLeft;
        NavigationStates.CROSSHAIR_COLOR.RightAction = NavigationStates.CROSSHAIR_COLOR.OptionRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_COLOR)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_COLOR.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_COLOR.ExecuteRight);

        foreach (var option in NavigationStates.CROSSHAIR_COLOR.Options)
        {
            ColorOptions.Add(option.Value, (SolidColorBrush)new BrushConverter().ConvertFromString(option.Value)!);
        }

        NavigationStates.CROSSHAIR_SIZE.SelectionLabel = "3";
        NavigationStates.CROSSHAIR_SIZE.Options = [new Option("1"), new Option("2"), new Option("3"), new Option("4"), new Option("5")];
        NavigationStates.CROSSHAIR_SIZE.SelectOption();
        NavigationStates.CROSSHAIR_SIZE.LeftAction = NavigationStates.CROSSHAIR_SIZE.OptionLeft;
        NavigationStates.CROSSHAIR_SIZE.RightAction = NavigationStates.CROSSHAIR_SIZE.OptionRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_SIZE)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_SIZE.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_SIZE.ExecuteRight);

        NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean = true;
        NavigationStates.CROSSHAIR_OUTLINE.LeftAction = NavigationStates.CROSSHAIR_OUTLINE.BooleanLeft;
        NavigationStates.CROSSHAIR_OUTLINE.RightAction = NavigationStates.CROSSHAIR_OUTLINE.BooleanRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_OUTLINE)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_OUTLINE.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_OUTLINE.ExecuteRight);

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

        var scale = int.Parse(NavigationStates.CROSSHAIR_SIZE.SelectionLabel);
        var color = ColorOptions[NavigationStates.CROSSHAIR_COLOR.SelectionLabel];

        if (!FormRenderFunctions.ContainsKey(NavigationStates.CROSSHAIR_FORM.SelectionLabel))
        {
            Debug.Print("Form is null {0}", NavigationStates.CROSSHAIR_FORM.SelectionLabel);
            return;
        }

        var formFunction = FormRenderFunctions[NavigationStates.CROSSHAIR_FORM.SelectionLabel];
        var geometryDrawing = GetGeometryDrawing(scale, color, formFunction);

        var screen = Screen.AllScreens.ElementAt(0);
        var dpiScale = 1 / screen.ScaleFactor;

        DrawingImage drawingImage = new(geometryDrawing);
        drawingImage.Freeze();

        Image image = new()
        {
            Source = drawingImage,
            Stretch = Stretch.None,
            RenderTransform = new ScaleTransform(dpiScale, dpiScale, drawingImage.Width / 2, drawingImage.Height / 2),
        };

        grid.Children.Clear();
        grid.Children.Add(image);

        UpdateAllOptions(scale, color, formFunction, dpiScale);
    }

    private void UpdateAllOptions(int scale, Brush color, Func<int, Brush, bool, Drawing> formFunction, double dpiScale)
    {
        foreach (var option in NavigationStates.CROSSHAIR_FORM.Options)
        {
            var optionFormFunction = FormRenderFunctions[option.Value];
            var brush = new SolidColorBrush(((SolidColorBrush)App.Current.FindResource("BrushSolidSuperBright")).Color);
            var geometryDrawing = GetGeometryDrawing(5, brush, optionFormFunction);

            DrawingImage drawingImage = new(geometryDrawing);
            drawingImage.Freeze();

            Image image = new()
            {
                Source = drawingImage,
                Stretch = Stretch.None,
                RenderTransform = new ScaleTransform(dpiScale, dpiScale, drawingImage.Width / 2, drawingImage.Height / 2),
                Margin = new Thickness(4, 0, 4, 0)
            };

            option.Image = image;
        }

        foreach (var option in NavigationStates.CROSSHAIR_COLOR.Options)
        {
            var colorOption = ColorOptions[option.Value];
            //var geometryDrawing = GetGeometryDrawing(5, colorOption, formFunction);


            GeometryGroup areaGroup = new();
            areaGroup.Children.Add(new RectangleGeometry(new Rect(-10, -10, 20, 20)));

            GeometryDrawing foregroundDrawing = new() { Geometry = areaGroup, Brush = colorOption };

            DrawingImage drawingImage = new(foregroundDrawing);
            drawingImage.Freeze();

            Image image = new()
            {
                Source = drawingImage,
                Stretch = Stretch.None,
                Margin = new Thickness(0, 2, 0, 0)
            };


            option.Image = image;
        }

        foreach (var option in NavigationStates.CROSSHAIR_SIZE.Options)
        {
            var sizeOption = int.Parse(option.Value);
            var geometryDrawing = GetGeometryDrawing(sizeOption, color, formFunction);

            DrawingImage drawingImage = new(geometryDrawing);
            drawingImage.Freeze();

            Image image = new()
            {
                Source = drawingImage,
                Stretch = Stretch.None,
                RenderTransform = new ScaleTransform(dpiScale, dpiScale, drawingImage.Width / 2, drawingImage.Height / 2),
                Margin = new Thickness(4, 0, 4, 0)
            };


            option.Image = image;
        }

    }

    private static Drawing GetGeometryDrawing(int scale, Brush color, Func<int, Brush, bool, Drawing> optionFormFunction)
    {
        var drawingWithFixedSize = (DrawingGroup)optionFormFunction(scale, color, NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean);

        GeometryGroup areaGroup = new();
        areaGroup.Children.Add(new RectangleGeometry(new Rect(-12, -12, 24, 24)));

        drawingWithFixedSize.Children.Insert(0, new GeometryDrawing() { Geometry = areaGroup, Brush = Brushes.Transparent });
        //        drawingWithFixedSize.Children.Insert(0, new GeometryDrawing() { Geometry = areaGroup, Brush = Brushes.Red });

        drawingWithFixedSize.Opacity = 0.8;

        return drawingWithFixedSize;
    }
}