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

        GeometryDrawing outlineDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = MakePen(outline)
        };

        GeometryDrawing foregroundDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(outlineDrawing);
        group.Children.Add(foregroundDrawing);

        return group;
    }

    public struct RingStruct
    {
        public double radiusOuter;
        public double radiusInner;
    }

    public static Drawing RenderRing(int size, Brush brush, bool outline)
    {
        var circleSizes = new Dictionary<int, RingStruct>
        {
            { 1, new() { radiusOuter = 2, radiusInner = 1 } },
            { 2, new() { radiusOuter = 2.6, radiusInner = 1.7 } },
            { 3, new() { radiusOuter = 4, radiusInner = 3.0 } },
            { 4, new() { radiusOuter = 6, radiusInner = 4.5 } },
            { 5, new() { radiusOuter = 10, radiusInner = 8.5 } },
        };
        var config = circleSizes[size];

        GeometryGroup foregroundGroup = new();
        foregroundGroup.Children.Add(new EllipseGeometry(new Point(0, 0), config.radiusOuter, config.radiusOuter));
        foregroundGroup.Children.Add(new EllipseGeometry(new Point(0, 0), config.radiusInner, config.radiusInner));

        GeometryGroup outlineGroup = new();
        outlineGroup.Children.Add(new EllipseGeometry(new Point(0, 0), config.radiusOuter, config.radiusOuter));
        if (size > 1)
            outlineGroup.Children.Add(new EllipseGeometry(new Point(0, 0), config.radiusInner, config.radiusInner));

        GeometryDrawing outlineDrawing = new()
        {
            Geometry = outlineGroup,
            Pen = MakePen(outline)
        };

        GeometryDrawing foregroundDrawing = new() { Geometry = foregroundGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(outlineDrawing);
        group.Children.Add(foregroundDrawing);

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

        var crosses = new Dictionary<int, CrossStruct>
        {
            {1, new() { offs = 0.5, length = 1, thickness = 1, centerSpace = 2, extraOffset = 1 } },
            {2, new() { offs = 0.5, length = 2, thickness = 1, centerSpace = 3, extraOffset = 1 } },
            {3, new() { offs = 0.5, length = 4, thickness = 1, centerSpace = 4, extraOffset = 1 } },
            {4, new() { offs = 0.0, length = 4, thickness = 2, centerSpace = 4, extraOffset = 0 } },
            {5, new() { offs = 0.0, length = 6, thickness = 2, centerSpace = 5, extraOffset = 0 } },
        };

        double offs = crosses[size].offs;
        double length = crosses[size].length;
        double thickness = crosses[size].thickness;
        double centerSpace = crosses[size].centerSpace;
        double extraOffset = crosses[size].extraOffset;

        GeometryGroup geometryGroup = new();

        // center
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(-1, -1, 2, 2)));
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 1, 1)));

        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-thickness / 2 + offs, -(length + centerSpace) + extraOffset, thickness, length)));
        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-thickness / 2 + offs, centerSpace, thickness, length)));

        geometryGroup.Children.Add(new RectangleGeometry(new Rect(centerSpace, -thickness / 2 + offs, length, thickness)));
        geometryGroup.Children.Add(new RectangleGeometry(new Rect(-(length + centerSpace) + extraOffset, -thickness / 2 + offs, length, thickness)));

        GeometryDrawing outlineDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = MakePen(outline)
        };

        GeometryDrawing foregroundDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(outlineDrawing);
        group.Children.Add(foregroundDrawing);

        return group;
    }

    public static Drawing RenderDiagonal(int size, Brush brush, bool outline)
    {
        GeometryGroup geometryGroup = new();

        if (size == 1)
        {
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-2, -2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(2, -2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(2, 2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-2, 2, 1, 1)));
        }
        else if (size == 2)
        {
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-2, -2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(2, -2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(2, 2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-2, 2, 1, 1)));

            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-3, -3, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(3, -3, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(3, 3, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-3, 3, 1, 1)));
        }
        else if (size >= 2)
        {
            DrawingGroup unRotatedCrossGroup = new();
            var rotated = (DrawingGroup)RenderCross(size, brush, outline);
            rotated.Transform = new RotateTransform(45);
            unRotatedCrossGroup.Children.Add(rotated);
            return unRotatedCrossGroup;
        }

        GeometryDrawing outlineDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = MakePen(outline)
        };

        GeometryDrawing foregroundDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(outlineDrawing);
        group.Children.Add(foregroundDrawing);

        return group;
    }

    public static Drawing ThreeDots(int size, Brush brush, bool outline)
    {
        GeometryGroup geometryGroup = new();

        // center
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(-1, -1, 2, 2)));
        //geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 1, 1)));

        if (size == 1)
        {
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, -2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-2, 2, 1, 1)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(2, 2, 1, 1)));
        }
        else if (size == 2)
        {
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-1, -3, 2, 2)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(-4, 3, 2, 2)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(2, 3, 2, 2)));
        }
        else if (size == 3)
        {
            geometryGroup.Children.Add(new EllipseGeometry(new Point(0, -5), 2, 2));
            geometryGroup.Children.Add(new EllipseGeometry(new Point(-5, 3), 2, 2));
            geometryGroup.Children.Add(new EllipseGeometry(new Point(5, 3), 2, 2));
        }
        else if (size == 4)
        {
            geometryGroup.Children.Add(new EllipseGeometry(new Point(0, -6), 3, 3));
            geometryGroup.Children.Add(new EllipseGeometry(new Point(-6, 4), 3, 3));
            geometryGroup.Children.Add(new EllipseGeometry(new Point(6, 4), 3, 3));
        }
        else if (size == 5)
        {
            geometryGroup.Children.Add(new EllipseGeometry(new Point(0, -7), 3.5, 3.5));
            geometryGroup.Children.Add(new EllipseGeometry(new Point(-7, 6), 3.5, 3.5));
            geometryGroup.Children.Add(new EllipseGeometry(new Point(7, 6), 3.5, 3.5));
        }

        GeometryDrawing outlineDrawing = new()
        {
            Geometry = geometryGroup,
            Pen = MakePen(outline)
        };

        GeometryDrawing foregroundDrawing = new() { Geometry = geometryGroup, Brush = brush, };

        DrawingGroup group = new();
        group.Children.Add(outlineDrawing);
        group.Children.Add(foregroundDrawing);

        return group;
    }

    private static Pen MakePen(bool outline)
    {
        return new Pen(new SolidColorBrush(Color.FromArgb(outline ? (byte)200 : (byte)0, 0, 0, 0)), 2);
    }
}

