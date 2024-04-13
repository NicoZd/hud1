using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
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
    public class CustomControl1 : Control
    {
        public Label LabelControl;

        public static DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(String), typeof(CustomControl1), new PropertyMetadata("Default", new PropertyChangedCallback(CustomTextBox_OnTextPropertyChanged)));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        static CustomControl1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));
        }

        private static void CustomTextBox_OnTextPropertyChanged(DependencyObject d,
                    DependencyPropertyChangedEventArgs e)
        {
            CustomControl1 customTextBox = d as CustomControl1;

            Debug.Print("xxx {0}", e.NewValue);

            customTextBox.Test();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LabelControl = Template.FindName("PART_Label", this) as Label;

            //Binding labelBinding = new Binding
            //{
            //    Path = new PropertyPath(nameof(Label)),
            //    Source = this
            //};

            //LabelControl.SetBinding(ContentControl.ContentProperty, labelBinding);

            LabelControl.Content = Label;
        }

        public void Test()
        {            
            if (LabelControl != null)
            {
                LabelControl.Content = Label;
            }
        }
    }
}
