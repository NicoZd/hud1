using CommunityToolkit.Mvvm.ComponentModel;
using CoreAudio;
using Hud1.Models;
using System.Text.RegularExpressions;
namespace Hud1.ViewModels
{
    public partial class AudioDeviceViewModel : ObservableObject
    {
        public static readonly float VOLUME_INCREMENT = 0.02f;

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

        [ObservableProperty]
        public Volume volume = new();


        private MMDeviceManager MMDeviceManager = new MMDeviceManager();

        public AudioDeviceViewModel()
        {
            MMDeviceManager.DevicesChanged += OnDevicesChanged;
            MMDeviceManager.VolumeChanged += OnVolumeChanged;
            OnDevicesChanged();
            OnVolumeChanged();
        }

        public void SelectPrevDevice()
        {
            MMDeviceManager.SelectPrevDevice();
        }

        public void SelectNextDevice()
        {
            MMDeviceManager.SelectNextDevice();
        }

        public void VolumeUp()
        {
            MMDeviceManager.SetVolume(Volume.Value + VOLUME_INCREMENT);
        }
        public void VolumeDown()
        {
            MMDeviceManager.SetVolume(Volume.Value - VOLUME_INCREMENT);
        }

        void OnVolumeChanged()
        {
            Volume = MMDeviceManager.GetVolume();
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
