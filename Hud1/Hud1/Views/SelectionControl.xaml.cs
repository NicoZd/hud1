using Hud1.Helpers;
using Hud1.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    public partial class SelectionControl : UserControl
    {
        private static readonly DependencyProperty NavigationStateProperty =
            BindingHelper.CreateProperty<SelectionControl, NavigationState>("NavigationState", null);

        public NavigationState NavigationState
        {
            get { return (NavigationState)GetValue(NavigationStateProperty); }
            set
            {
                SetValue(NavigationStateProperty, value);
                Debug.Print("SelectionControl SET {0} {1}", NavigationState?.Name, NavigationState?.Label);
            }
        }

        public SelectionControl()
        {
            InitializeComponent();
            Debug.Print("SelectionControl {0} {1}", NavigationState?.Name, NavigationState?.Label);
        }
    }
}
