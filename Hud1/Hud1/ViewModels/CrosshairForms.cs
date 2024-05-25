using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels;

class CrosshairForms
{
    public static Drawing RenderDot(int size, Brush brush, bool outline)
    {
        var geoms = new Dictionary<int, Geometry>
        {
            { 1, new RectangleGeometry(new Rect(0, 0, 1, 1))},
            { 2, new RectangleGeometry(new Rect(-1, -1, 2, 2))},
            { 3, new EllipseGeometry(new Point(0, 0), 2, 2)},
            { 4, new EllipseGeometry(new Point(0, 0), 3, 3)},
            { 5, new EllipseGeometry(new Point(0, 0), 6, 6)},
        };

        GeometryGroup geometryGroup = new();
        geometryGroup.Children.Add(geoms[size]);

        GeometryDrawing foregroundDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(125, 0, 0, 0)), outline ? 2 : 0)
        };

        GeometryDrawing outlineDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(foregroundDrawing);
        group.Children.Add(outlineDrawing);

        return group;
    }

    public struct CircleStruct
    {
        public double radiusOuter;
        public double radiusInner;
    }
    public static Drawing RenderCircle(int size, Brush brush, bool outline)
    {
        var circleSizes = new Dictionary<int, CircleStruct>
        {
            { 1, new() { radiusOuter = 2.4, radiusInner = 1.8 } },
            { 2, new() { radiusOuter = 2.6, radiusInner = 1.7 } },
            { 3, new() { radiusOuter = 4, radiusInner = 3.0 } },
            { 4, new() { radiusOuter = 6, radiusInner = 4.5 } },
            { 5, new() { radiusOuter = 10, radiusInner = 8.5 } },
        };
        var config = circleSizes[size];

        GeometryGroup geometryGroup = new();
        geometryGroup.Children.Add(new EllipseGeometry(new Point(0, 0), config.radiusOuter, config.radiusOuter));
        geometryGroup.Children.Add(new EllipseGeometry(new Point(0, 0), config.radiusInner, config.radiusInner));

        GeometryDrawing foregroundDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(125, 0, 0, 0)), outline ? 2 : 0)
        };

        GeometryDrawing outlineDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(foregroundDrawing);
        group.Children.Add(outlineDrawing);

        return group;
    }

    public struct CrossStruct
    {
        public double offs;
        public double length;
        public double thickness;
        public double centerSpace;
        public double extraOffset;
    }
    public static Drawing RenderCross(int size, Brush brush, bool outline)
    {

        var circleSizes = new Dictionary<int, CrossStruct>
        {
            {1, new() { offs = 0.5, length = 1, thickness = 1, centerSpace = 2, extraOffset = 1 } },
            {2, new() { offs = 0.5, length = 2, thickness = 1, centerSpace = 3, extraOffset = 1 } },
            {3, new() { offs = 0.5, length = 4, thickness = 1, centerSpace = 4, extraOffset = 1 } },
            {4, new() { offs = 0.0, length = 4, thickness = 2, centerSpace = 4, extraOffset = 0 } },
            {5, new() { offs = 0.0, length = 6, thickness = 2, centerSpace = 5, extraOffset = 0 } },
        };

        double offs = circleSizes[size].offs;
        double length = circleSizes[size].length;
        double thickness = circleSizes[size].thickness;
        double centerSpace = circleSizes[size].centerSpace;
        double extraOffset = circleSizes[size].extraOffset;

        GeometryGroup geometryGroup = new();

        // center
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(-1, -1, 2, 2)));
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 1, 1)));

        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-thickness / 2 + offs, -(length + centerSpace) + extraOffset, thickness, length)));
        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-thickness / 2 + offs, centerSpace, thickness, length)));

        geometryGroup.Children.Add(new RectangleGeometry(new Rect(centerSpace, -thickness / 2 + offs, length, thickness)));
        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-(length + centerSpace) + extraOffset, -thickness / 2 + offs, length, thickness)));

        GeometryDrawing foregroundDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = new Pen(new SolidColorBrush(Color.FromArgb(125, 0, 0, 0)), outline ? 2 : 0)
        };

        GeometryDrawing outlineDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(foregroundDrawing);
        group.Children.Add(outlineDrawing);

        return group;
    }
}

