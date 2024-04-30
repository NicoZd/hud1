using Hud1.Helpers;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    /// <summary>
    /// Interaction logic for MacrosView.xaml
    /// </summary>
    public partial class MacrosView : UserControl
    {
        private static readonly DependencyProperty MacrosViewModelProperty =
           BindingHelper.CreateProperty<MacrosView, MacrosViewModel>("MacrosViewModel", null);

        public MacrosViewModel MacrosViewModel
        {
            get { return (MacrosViewModel)GetValue(MacrosViewModelProperty); }
            set { SetValue(MacrosViewModelProperty, value); }
        }

        public MacrosView()
        {
            InitializeComponent();
        }
    }
}
