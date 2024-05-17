using Hud1.Helpers;
using Hud1.Models;
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

namespace Hud1.Views
{
    public partial class BooleanPanel : UserControl
    {
        private static readonly DependencyProperty NavigationStateProperty =
             BindingHelper.CreateProperty<BooleanPanel, NavigationState>("NavigationState", null);

        public NavigationState NavigationState
        {
            get { return (NavigationState)GetValue(NavigationStateProperty); }
            set { SetValue(NavigationStateProperty, value); }
        }

        public BooleanPanel()
        {
            InitializeComponent();
        }
    }
}
