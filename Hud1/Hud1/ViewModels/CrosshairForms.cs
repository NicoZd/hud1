using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels;

class CrosshairForms
{
    public static Drawing RenderDot(double scale, Brush brush, bool outline)
    {
        var size = 8 * scale;
        var center = new Point(0, 0);

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

    public static Drawing RenderCircle(double scale, Brush brush, bool outline)
    {
        var size = 8 * scale;
        var center = new Point(0, 0);

        GeometryGroup geometryGroup = new();
        geometryGroup.Children.Add(new EllipseGeometry(center, size / 2, size / 2));
        geometryGroup.Children.Add(new EllipseGeometry(center, size / 2 - 2.5, size / 2 - 2.5));

        GeometryDrawing aGeometryDrawing = new()
        {
            Geometry = geometryGroup,
            Brush = brush,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)), outline ? 1.1 : 0)
        };

        return aGeometryDrawing;
    }

    public static Drawing RenderCross(double scale, Brush brush, bool outline)
    {
        var scaleOffset = 1;
        var length = 4 * scale * scaleOffset;
        var thickness = 1 * scale * scaleOffset;
        var centerSpace = 4 * scale * scaleOffset;

        GeometryGroup geometryGroup = new();

        // center
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 1, 1)));

        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-thickness / 2 + 0.5, -(length + centerSpace) + 1, thickness, length)));
        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-thickness / 2 + 0.5, centerSpace, thickness, length)));

        geometryGroup.Children.Add(new RectangleGeometry(new Rect(centerSpace, -thickness / 2 + 0.5, length, thickness)));
        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-(length + centerSpace) + 1, -thickness / 2 + 0.5, length, thickness)));

        GeometryDrawing aGeometryDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(125, 0, 0, 0)), outline ? 2 : 0)
        };

        GeometryDrawing aGeometryDrawing2 = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new DrawingGroup();
        group.Children.Add(aGeometryDrawing);
        group.Children.Add(aGeometryDrawing2);

        return group;
    }
}

