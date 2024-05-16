using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static Hud1.Views.TouchScrolling;

namespace Hud1.Views
{
    public class Aquarium : UIElement
    {
        public static readonly DependencyProperty HasFishProperty =
            DependencyProperty.RegisterAttached(
          "HasFish",
          typeof(bool),
          typeof(Aquarium),
          new FrameworkPropertyMetadata(defaultValue: false,
              flags: FrameworkPropertyMetadataOptions.AffectsRender,
              OnChange
              )
        );

        private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                ((FrameworkElement)d).BringIntoView(new Rect(0, -100, 0, 200));
            }
        }

        public static bool GetHasFish(UIElement target) =>
            (bool)target.GetValue(HasFishProperty);

        public static void SetHasFish(UIElement target, bool value) =>
            target.SetValue(HasFishProperty, value);
    }
}
