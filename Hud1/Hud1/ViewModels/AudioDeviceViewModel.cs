using CommunityToolkit.Mvvm.ComponentModel;
using CoreAudio;
using Hud1.Models;
namespace Hud1.ViewModels
{
    public partial class AudioDeviceViewModel : ObservableObject
    {
        [ObservableProperty]
        public List<MMDevice> playbackDevices = [];

        [ObservableProperty]
        public String defaultPlaybackDeviceID = "";

        [ObservableProperty]
        public List<MMDevice> captureDevices = [];

        [ObservableProperty]
        public String defaultCaptureDeviceID = "";

        private MMDeviceManager MMDeviceManager = new MMDeviceManager();

        public AudioDeviceViewModel()
        {
            MMDeviceManager.DevicesChanged += OnDevicesChanged;
            OnDevicesChanged();
        }

        void OnDevicesChanged()
        {
            PlaybackDevices = MMDeviceManager.PlaybackDevices;
            CaptureDevices = MMDeviceManager.CaptureDevices;
            DefaultPlaybackDeviceID = MMDeviceManager.DefaultPlaybackDeviceId;
            DefaultCaptureDeviceID = MMDeviceManager.DefaultCaptureDeviceId;
        }

    }
}
