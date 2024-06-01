using System.Windows;
using System.Windows.Controls;

namespace Hud1.Controls;

internal class IgnoreWidthControl : ContentControl
{
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (sizeInfo.WidthChanged)
            InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        constraint.Width = ActualWidth;
        var size = new Size();
        var child = GetFirstVisualChild();
        if (child != null)
        {
            child.Measure(constraint);
            size.Height = child.DesiredSize.Height;
        }

        return size;
    }

    private UIElement? GetFirstVisualChild()
    {
        return VisualChildrenCount <= 0 ? null : GetVisualChild(0) as UIElement;
    }
}