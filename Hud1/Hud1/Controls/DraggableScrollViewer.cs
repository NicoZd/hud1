using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hud1.Controls;

internal class DraggableScrollViewer : ScrollViewer
{
    private bool isPressed = false;
    private DateTime pressStartTime;
    private const int longPressDurationMilliseconds = 25;
    private Point startPoint;
    private bool isDragging = false;
    private double virtualVerticalOffset = 0;

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        base.OnPreviewMouseMove(e);
        if (isPressed)
        {
            var pressDuration = DateTime.Now - pressStartTime;
            if (pressDuration.TotalMilliseconds >= longPressDurationMilliseconds)
            {
                CaptureMouse();
                if (isDragging && e.LeftButton == MouseButtonState.Pressed)
                {
                    var currentPoint = e.GetPosition(this);
                    var offset = startPoint - currentPoint;
                    virtualVerticalOffset += offset.Y;
                    virtualVerticalOffset = Math.Min(Math.Max(virtualVerticalOffset, 0), ScrollableHeight);
                    startPoint = currentPoint;
                }
            }
        }
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonUp(e);
        isDragging = false;
        isPressed = false;
        ReleaseMouseCapture();
        CompositionTarget.Rendering -= UpdatePosition;

    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);
        var point = e.GetPosition(this);
        if (point.X > ActualWidth - 25)
            return;

        startPoint = point;
        isDragging = true;
        pressStartTime = DateTime.Now;
        isPressed = true;
        virtualVerticalOffset = VerticalOffset;
        CompositionTarget.Rendering += UpdatePosition;
    }

    private void UpdatePosition(object? sender, EventArgs e)
    {
        var target = VerticalOffset + (virtualVerticalOffset - VerticalOffset) * 0.3;
        ScrollToVerticalOffset(target);
    }

}
