using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    /// <summary>
    /// Content control ignoring the content width and setting the content MaxWidth to the actual width.
    /// Suitable to force a TextBlock to wrap the text.
    /// </summary>
    public class IgnoreWidthControl : ContentControl
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
            Size size = new Size();
            UIElement child = GetFirstVisualChild();
            if (child != null)
            {
                child.Measure(constraint);
                size.Height = child.DesiredSize.Height;
            }

            return size;
        }

        private UIElement GetFirstVisualChild()
        {
            if (this.VisualChildrenCount <= 0)
                return null;
            return this.GetVisualChild(0) as UIElement;
        }
    }
}