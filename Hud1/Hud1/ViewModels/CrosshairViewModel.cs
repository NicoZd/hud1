using Hud1.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfScreenHelper;

namespace Hud1.ViewModels;

public class PixelatedImage : FrameworkElement
{
    public Drawing? Drawing { get; set; }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var visual = new DrawingVisual();
        using (DrawingContext dc = visual.RenderOpen())
        {
            dc.DrawDrawing(Drawing);
            dc.Close();
        }

        visual.Transform = new TranslateTransform(25, 25);
        var size = 50;
        var dpi = 96;

        RenderTargetBitmap target = new RenderTargetBitmap(size, size, dpi, dpi, PixelFormats.Pbgra32);
        target.Render(visual);

        int stride = (int)target.PixelWidth * (target.Format.BitsPerPixel / 8);
        byte[] pixels = new byte[(int)target.PixelHeight * stride];
        target.CopyPixels(pixels, stride, 0);

        int scaleFactor = 7;

        for (int y = 0; y < target.PixelHeight; y++)
        {
            for (int x = 0; x < target.PixelWidth; x++)
            {
                var offset = (y * target.PixelWidth + x) * 4;

                var drawBackground = true;
                if (drawBackground)
                {
                    drawingContext.DrawRectangle( //bgra
                        new SolidColorBrush(Color.FromArgb(255, 60, 60, 60)),
                        null,
                        new Rect(x * scaleFactor, y * scaleFactor, scaleFactor, scaleFactor)
                        );
                }

                drawingContext.DrawRectangle( //bgra
                    new SolidColorBrush(Color.FromArgb(
                        pixels[offset + 3], pixels[offset + 2], pixels[offset + 1], pixels[offset + 0]
                        )),
                    null,
                    new Rect(x * scaleFactor, y * scaleFactor, scaleFactor, scaleFactor)
                    );
            }
        }
    }
}

public class CrosshairViewModel
{
    public static readonly CrosshairViewModel Instance = new();

    public static readonly List<string> Forms = ["a", "b", "c"];

    private CrosshairViewModel() { }

    public void BuildNavigation()
    {
        var Navigation = NavigationViewModel.Instance.Navigation;

        NavigationStates.CROSSHAIR_ENABLED.LeftAction = NavigationStates.CROSSHAIR_ENABLED.BooleanLeft;
        NavigationStates.CROSSHAIR_ENABLED.RightAction = NavigationStates.CROSSHAIR_ENABLED.BooleanRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_ENABLED)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_ENABLED.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_ENABLED.ExecuteRight);

        NavigationStates.CROSSHAIR_FORM.SelectionLabel = "cross";
        NavigationStates.CROSSHAIR_FORM.Options = ["dot", "circle", "cross", "diagonal", "3 dots"];
        NavigationStates.CROSSHAIR_FORM.LeftAction = NavigationStates.CROSSHAIR_FORM.OptionLeft;
        NavigationStates.CROSSHAIR_FORM.RightAction = NavigationStates.CROSSHAIR_FORM.OptionRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_FORM)
          .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_FORM.ExecuteLeft)
          .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_FORM.ExecuteRight);

        NavigationStates.CROSSHAIR_COLOR.SelectionLabel = "white";
        NavigationStates.CROSSHAIR_COLOR.Options = ["red", "green", "blue", "white"];
        NavigationStates.CROSSHAIR_COLOR.LeftAction = NavigationStates.CROSSHAIR_COLOR.OptionLeft;
        NavigationStates.CROSSHAIR_COLOR.RightAction = NavigationStates.CROSSHAIR_COLOR.OptionRight;
        Navigation.Configure(NavigationStates.CROSSHAIR_COLOR)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_COLOR.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_COLOR.ExecuteRight);

        NavigationStates.CROSSHAIR_SIZE.SelectionLabel = "1";
        NavigationStates.CROSSHAIR_SIZE.Options = ["1", "2", "3"];
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

        var colors = new Dictionary<string, Brush>
        {
            { "red", Brushes.Red },
            { "green", (SolidColorBrush)new BrushConverter().ConvertFromString("#ff00ff00")! },
            { "blue", Brushes.Blue },
            { "white", Brushes.White }
        };

        var scale = Double.Parse(NavigationStates.CROSSHAIR_SIZE.SelectionLabel);
        var color = colors[NavigationStates.CROSSHAIR_COLOR.SelectionLabel];

        var renderFunctions = new Dictionary<string, Func<double, Brush, bool, Drawing>>
        {
            { "dot", CrosshairForms.RenderDot },
            { "circle", CrosshairForms.RenderCircle },
            { "cross", CrosshairForms.RenderCross },
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

        var screen = Screen.AllScreens.ElementAt(0);
        var dpiScale = 1 / screen.ScaleFactor;

        DrawingImage drawingImage = new(geometryDrawing);
        drawingImage.Freeze();

        Image image = new()
        {
            Source = drawingImage,
            Stretch = Stretch.None,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            RenderTransform = new ScaleTransform(dpiScale, dpiScale, drawingImage.Width / 2, drawingImage.Width / 2),
        };
        image.SnapsToDevicePixels = true;

        grid.Children.Clear();

        var renderBackground = true;
        if (renderBackground)
        {
            PixelatedImage debugImage = new()
            {
                Drawing = drawingImage.Drawing,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(100, -300, 0, 0),
                RenderTransform = new ScaleTransform(dpiScale, dpiScale, 0, 0),
            };
            grid.Children.Add(debugImage);
        }

        grid.Children.Add(image);
    }
}