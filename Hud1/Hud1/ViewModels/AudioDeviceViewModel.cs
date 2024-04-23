using CommunityToolkit.Mvvm.ComponentModel;
using CoreAudio;
using Hud1.Models;
using System.Text.RegularExpressions;
namespace Hud1.ViewModels
{
    public partial class AudioDeviceViewModel : ObservableObject
    {
        [ObservableProperty]
        public List<MMDevice> playbackDevices = [];

        [ObservableProperty]
        public String defaultPlaybackDeviceID = "";

        [ObservableProperty]
        public String defaultPlaybackDeviceName = "";

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

        public void SelectPrevDevice()
        {
            MMDeviceManager.SelectPrevDevice();
        }

        public void SelectNextDevice()
        {
            MMDeviceManager.SelectNextDevice();
        }


        void OnDevicesChanged()
        {
            PlaybackDevices = MMDeviceManager.PlaybackDevices;
            CaptureDevices = MMDeviceManager.CaptureDevices;
            DefaultPlaybackDeviceID = MMDeviceManager.DefaultPlaybackDeviceId;
            DefaultCaptureDeviceID = MMDeviceManager.DefaultCaptureDeviceId;

            var playbackDevice = PlaybackDevices.Find(d => d.ID == DefaultPlaybackDeviceID);
            DefaultPlaybackDeviceName = playbackDevice == null ? "Unknown" : TrimName(playbackDevice.DeviceInterfaceFriendlyName);
        }

        private static string TrimName(string name)
        {
            name = Regex.Replace(name, @"^\d+-\s", "");
            return name;
        }

    }
}
