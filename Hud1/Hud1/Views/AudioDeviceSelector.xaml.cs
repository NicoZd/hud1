using Hud1.Helpers;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    public partial class AudioDeviceSelector : UserControl
    {
        public AudioDeviceSelectorViewModel ViewModel = new AudioDeviceSelectorViewModel();

        private static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AudioDeviceSelectorViewModel), typeof(AudioDeviceSelector));

        public static readonly DependencyProperty LabelProperty =
            BindingHelper.CreateProperty<AudioDeviceSelector, string>("Label", "",
                (control, value) => control.ViewModel.Label = value);


        public static readonly DependencyProperty SelectedProperty =
            BindingHelper.CreateProperty<AudioDeviceSelector, bool>("Selected", false,
                (control, value) => control.ViewModel.Selected = value);

        public String Label { set => SetValue(LabelProperty, value); }

        public bool Selected { set => SetValue(SelectedProperty, value); }

        public AudioDeviceSelector()
        {
            InitializeComponent();
            SetValue(ViewModelProperty, ViewModel);
        }
    }
}
