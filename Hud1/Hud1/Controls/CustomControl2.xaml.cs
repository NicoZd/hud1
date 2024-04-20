using System.Windows;
using System.Windows.Controls;

namespace Hud1.Controls
{
    public partial class CustomControl2 : UserControl
    {
        public CustomControl2Model Model = new CustomControl2Model();

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label",
          typeof(String),
          typeof(CustomControl2),
          new PropertyMetadata("", new PropertyChangedCallback(OnLabelChanged))
        );

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
          "Selected",
          typeof(bool),
          typeof(CustomControl2),
          new PropertyMetadata(false, new PropertyChangedCallback(OnSelectedChanged))
        );

        public static void OnLabelChanged(DependencyObject send, DependencyPropertyChangedEventArgs args)
        {
            (send as CustomControl2).Model.Label = (string)args.NewValue;
        }

        public static void OnSelectedChanged(DependencyObject send, DependencyPropertyChangedEventArgs args)
        {
            (send as CustomControl2).Model.Selected = (bool)args.NewValue;
        }

        public String Label
        {
            get => (String)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        public CustomControl2()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this.Model;
        }



    }
}
