using CommunityToolkit.Mvvm.ComponentModel;
using CoreAudio;
using Hud1.Models;
using System.Text.RegularExpressions;
namespace Hud1.ViewModels
{
    public partial class AudioDeviceViewModel : ObservableObject
    {
        public static readonly float VOLUME_INCREMENT_SINGLE = 0.01f;
        public static readonly float VOLUME_INCREMENT_REPEAT = 0.04f;

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
            if (NavigationState.Repeat)
            {
                MMDeviceManager.SetVolume(Volume.Value + VOLUME_INCREMENT_REPEAT);
            }
            else
            {
                MMDeviceManager.SetVolume(Volume.Value + VOLUME_INCREMENT_SINGLE);
            }
        }
        public void VolumeDown()
        {
            if (NavigationState.Repeat)
            {
                MMDeviceManager.SetVolume(Volume.Value - VOLUME_INCREMENT_REPEAT);
            }
            else
            {
                MMDeviceManager.SetVolume(Volume.Value - VOLUME_INCREMENT_SINGLE);
            }
        }

        void OnVolumeChanged()
        {
            Volume = MMDeviceManager.GetVolume();
            NavigationStates.PLAYBACK_VOLUME.SelectionLabel = "" + Math.Round(Volume.Value * 100);
            NavigationStates.PLAYBACK_MUTE.SelectionLabel = Volume.Muted ? "Muted" : "Unmuted";
        }


        void OnDevicesChanged()
        {
            PlaybackDevices = MMDeviceManager.PlaybackDevices;
            CaptureDevices = MMDeviceManager.CaptureDevices;
            DefaultPlaybackDeviceID = MMDeviceManager.DefaultPlaybackDeviceId;
            DefaultCaptureDeviceID = MMDeviceManager.DefaultCaptureDeviceId;

            var playbackDeviceIndex = PlaybackDevices.FindIndex(d => d.ID == DefaultPlaybackDeviceID);
            if (playbackDeviceIndex == -1)
                return;
            var playbackDevice = PlaybackDevices[playbackDeviceIndex];
            DefaultPlaybackDeviceName = playbackDevice == null ? "Unknown" : TrimName(playbackDevice.DeviceInterfaceFriendlyName);

            NavigationStates.PLAYBACK_DEVICE.SelectionLabel = DefaultPlaybackDeviceName + " " + (playbackDeviceIndex + 1) + "/" + PlaybackDevices.Count;
            //NavigationStates.PLAYBACK_DEVICE.Selection
        }

        private static string TrimName(string name)
        {
            name = Regex.Replace(name, @"^\d+-\s", "");
            return name;
        }

    }
}
