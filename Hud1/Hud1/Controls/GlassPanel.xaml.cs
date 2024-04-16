using System;
using System.Collections.Generic;
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
    public partial class GlassPanel : UserControl
    {
        //public static readonly DependencyProperty PlaceHolder1Property = DependencyProperty.Register("PlaceHolder1", typeof(object), typeof(GlassPanel), new UIPropertyMetadata(null));
        //public object PlaceHolder1
        //{
        //    get { return (object)GetValue(PlaceHolder1Property); }
        //    set { SetValue(PlaceHolder1Property, value); }
        //}

        public GlassPanel()
        {
            InitializeComponent();
        }
    }
}
