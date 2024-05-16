using System.Windows;

namespace Hud1.Views
{
    public class BringIntoView : UIElement
    {
        public static readonly DependencyProperty BringIntoViewProperty =
            DependencyProperty.RegisterAttached(
                "BringIntoView",
                typeof(bool),
                typeof(BringIntoView),
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

        public static bool GetBringIntoView(UIElement target) =>
            (bool)target.GetValue(BringIntoViewProperty);

        public static void SetBringIntoView(UIElement target, bool value) =>
            target.SetValue(BringIntoViewProperty, value);
    }
}
