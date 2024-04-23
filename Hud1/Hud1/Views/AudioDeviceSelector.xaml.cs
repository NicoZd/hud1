using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    [INotifyPropertyChanged]
    public partial class AudioDeviceSelector : UserControl
    {
        [ObservableProperty]
        public AudioDeviceSelectorViewModel viewModel = new AudioDeviceSelectorViewModel();

        private static readonly DependencyProperty ModellPropertyProperty =
            DependencyProperty.Register("ViewModel", typeof(AudioDeviceSelectorViewModel), typeof(AudioDeviceSelector));

        public static readonly DependencyProperty HudStateProperty =
            BindingHelper.CreateProperty<AudioDeviceSelector, NavigationState>("HudState", null, (a, b) =>
            {
                if (b.SelectRight)
                {
                    Debug.Print("SelectRight");
                }
            });

        public NavigationState HudState
        {
            set => SetValue(HudStateProperty, value);
            get => (NavigationState)GetValue(HudStateProperty);
        }

        public AudioDeviceSelector()
        {
            InitializeComponent();
            Debug.Print("AudioDeviceSelector {0} {1}", HudState?.Selected, HudState?.Visibility);
        }
    }
}
