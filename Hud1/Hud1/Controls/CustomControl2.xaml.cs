using Hud1.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hud1.Controls
{
    public partial class CustomControl2 : UserControl
    {
        CustomControl2Model model = new CustomControl2Model();

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
            (send as CustomControl2).model.Label = (string)args.NewValue;
        }

        public static void OnSelectedChanged(DependencyObject send, DependencyPropertyChangedEventArgs args)
        {
            (send as CustomControl2).model.Selected = (bool)args.NewValue;
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
            this.DataContext = model;
        }



    }
}
