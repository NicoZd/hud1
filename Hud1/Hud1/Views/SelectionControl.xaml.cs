using Hud1.Helpers;
using Hud1.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hud1.Views
{
    public partial class SelectionControl : UserControl
    {
        private static readonly DependencyProperty NavigationStateProperty =
            BindingHelper.CreateProperty<SelectionControl, NavigationState>("NavigationState", null);

        public NavigationState NavigationState
        {
            get { return (NavigationState)GetValue(NavigationStateProperty); }
            set { SetValue(NavigationStateProperty, value); }
        }

        public SelectionControl()
        {
            InitializeComponent();
        }
    }
}
