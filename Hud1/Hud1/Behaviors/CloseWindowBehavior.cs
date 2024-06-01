using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
using System.Windows;

namespace Hud1.Behaviors;

internal class CloseWindowBehavior : Behavior<Window>
{
    internal bool IsCloseActivated
    {
        get => (bool)GetValue(IsCloseActivatedProperty);
        set => SetValue(IsCloseActivatedProperty, value);
    }

    private static readonly DependencyProperty IsCloseActivatedProperty =
        DependencyProperty.Register("IsCloseActivated", typeof(bool), typeof(CloseWindowBehavior), new PropertyMetadata(false, OnIsCloseActivatedChanged));

    private static void OnIsCloseActivatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //Debug.Print($"OnIsCloseActivatedChanged: {(bool)e.NewValue}");
        if (d is CloseWindowBehavior behavior && behavior.AssociatedObject != null)
        {
            if ((bool)e.NewValue)
            {
                Debug.Print($"CloseWindowBehavior Close {behavior.AssociatedObject} {Entry.Millis()}");
                behavior.AssociatedObject.Close();
            };
        }
    }
}
