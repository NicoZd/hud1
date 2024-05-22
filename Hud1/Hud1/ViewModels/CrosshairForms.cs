using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels;

class CrosshairForms
{
    public static GeometryDrawing RenderDot(double scale, Brush brush, bool outline)
    {
        var size = 8 * scale;
        var center = new Point(size / 2, size / 2);

        GeometryGroup geometryGroup = new();
        geometryGroup.Children.Add(new EllipseGeometry(center, size / 2, size / 2));

        GeometryDrawing aGeometryDrawing = new()
        {
            Geometry = geometryGroup,
            Brush = brush,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)), outline ? 1.1 : 0)
        };

        return aGeometryDrawing;
    }

    public static GeometryDrawing RenderCircle(double scale, Brush brush, bool outline)
    {
        var size = 8 * scale;
        var center = new Point(size / 2, size / 2);

        GeometryGroup geometryGroup = new();
        geometryGroup.Children.Add(new EllipseGeometry(center, size / 2, size / 2));
        geometryGroup.Children.Add(new EllipseGeometry(center, size / 2 - 2.0, size / 2 - 2.0));

        GeometryDrawing aGeometryDrawing = new()
        {
            Geometry = geometryGroup,
            Brush = brush,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)), outline ? 0.5 : 0)
        };

        return aGeometryDrawing;
    }
}

