using CommunityToolkit.Mvvm.ComponentModel;
using CoreAudio;
using Hud1.Models;
namespace Hud1.ViewModels
{
    public partial class HudViewModel : ObservableObject
    {
        [ObservableProperty]
        public String? state;

        [ObservableProperty]
        public Dictionary<string, object> states = new Dictionary<string, object> { };

        [ObservableProperty]
        public List<MMDevice> playbackDevices = [];

        [ObservableProperty]
        public String defaultPlaybackDeviceID = "";

        [ObservableProperty]
        public List<MMDevice> captureDevices = [];

        [ObservableProperty]
        public String defaultCaptureDeviceID = "";

        public MMDeviceManager MMDeviceManager = new MMDeviceManager();

        public HudViewModel()
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
