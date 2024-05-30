using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Hud1.Controls
{
    class CloseWindowBehavior : Behavior<Window>
    {
        public bool IsCloseActivated
        {
            get { return (bool)GetValue(IsCloseActivatedProperty); }
            set { SetValue(IsCloseActivatedProperty, value); }
        }

        public static readonly DependencyProperty IsCloseActivatedProperty =
            DependencyProperty.Register("IsCloseActivated", typeof(bool), typeof(CloseWindowBehavior), new PropertyMetadata(false, OnIsCloseActivatedChanged));

        private static void OnIsCloseActivatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"OnIsCloseActivatedChanged: {(bool)e.NewValue}");
            if (d is CloseWindowBehavior behavior && behavior.AssociatedObject != null)
            {
                if ((bool)e.NewValue)
                {
                    Debug.Print($"Call close on: {behavior.AssociatedObject}");

                    behavior.AssociatedObject.Close();
                };
            }
        }

    }
}
