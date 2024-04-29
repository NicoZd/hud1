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
        public String defaultCaptureDeviceName = "";

        [ObservableProperty]
        public Volume playbackVolume = new();

        [ObservableProperty]
        public Volume captureVolume = new();


        private MMDeviceManager MMDeviceManager = new MMDeviceManager();

        public AudioDeviceViewModel()
        {
            MMDeviceManager.DevicesChanged += OnDevicesChanged;
            MMDeviceManager.VolumeChanged += OnVolumeChanged;
            OnDevicesChanged();
            OnVolumeChanged();
        }

        public void SelectPrevPlaybackDevice()
        {
            MMDeviceManager.SelectPrevPlaybackDevice();
        }

        public void SelectNextPlaybackDevice()
        {
            MMDeviceManager.SelectNextPlaybackDevice();
        }

        public void PlaybackVolumeUp()
        {
            if (NavigationState.Repeat)
            {
                MMDeviceManager.SetPlaybackVolume(PlaybackVolume.Value + VOLUME_INCREMENT_REPEAT);
            }
            else
            {
                MMDeviceManager.SetPlaybackVolume(PlaybackVolume.Value + VOLUME_INCREMENT_SINGLE);
            }
        }
        public void PlaybackVolumeDown()
        {
            if (NavigationState.Repeat)
            {
                MMDeviceManager.SetPlaybackVolume(PlaybackVolume.Value - VOLUME_INCREMENT_REPEAT);
            }
            else
            {
                MMDeviceManager.SetPlaybackVolume(PlaybackVolume.Value - VOLUME_INCREMENT_SINGLE);
            }
        }
        public void PlaybackMute()
        {
            MMDeviceManager.SetPlaybackMute(true);
        }

        public void PlaybackUnmute()
        {
            MMDeviceManager.SetPlaybackMute(false);
        }

        public void SelectPrevCaptureDevice()
        {
            MMDeviceManager.SelectPrevCaptureDevice();
        }

        public void SelectNextCaptureDevice()
        {
            MMDeviceManager.SelectNextCaptureDevice();
        }

        public void CaptureVolumeUp()
        {
            if (NavigationState.Repeat)
            {
                MMDeviceManager.SetCaptureVolume(CaptureVolume.Value + VOLUME_INCREMENT_REPEAT);
            }
            else
            {
                MMDeviceManager.SetCaptureVolume(CaptureVolume.Value + VOLUME_INCREMENT_SINGLE);
            }
        }
        public void CaptureVolumeDown()
        {
            if (NavigationState.Repeat)
            {
                MMDeviceManager.SetCaptureVolume(CaptureVolume.Value - VOLUME_INCREMENT_REPEAT);
            }
            else
            {
                MMDeviceManager.SetCaptureVolume(CaptureVolume.Value - VOLUME_INCREMENT_SINGLE);
            }
        }
        public void CaptureMute()
        {
            MMDeviceManager.SetCaptureMute(true);
        }

        public void CaptureUnmute()
        {
            MMDeviceManager.SetCaptureMute(false);
        }

        void OnVolumeChanged()
        {
            PlaybackVolume = MMDeviceManager.GetPlaybackVolume();
            NavigationStates.PLAYBACK_VOLUME.SelectionLabel = "" + Math.Round(PlaybackVolume.Value * 100);
            NavigationStates.PLAYBACK_MUTE.SelectionLabel = PlaybackVolume.Muted ? "Muted" : "Unmuted";

            CaptureVolume = MMDeviceManager.GetCaptureVolume();
            NavigationStates.CAPTURE_VOLUME.SelectionLabel = "" + Math.Round(CaptureVolume.Value * 100);
            NavigationStates.CAPTURE_MUTE.SelectionLabel = CaptureVolume.Muted ? "Muted" : "Unmuted";
        }


        void OnDevicesChanged()
        {
            PlaybackDevices = MMDeviceManager.PlaybackDevices;
            DefaultPlaybackDeviceID = MMDeviceManager.DefaultPlaybackDeviceId;

            var playbackDeviceIndex = PlaybackDevices.FindIndex(d => d.ID == DefaultPlaybackDeviceID);
            if (playbackDeviceIndex != -1)
            {
                var playbackDevice = PlaybackDevices[playbackDeviceIndex];
                DefaultPlaybackDeviceName = playbackDevice == null ? "Unknown" : TrimName(playbackDevice.DeviceInterfaceFriendlyName);
                NavigationStates.PLAYBACK_DEVICE.SelectionLabel = DefaultPlaybackDeviceName + " " + (playbackDeviceIndex + 1) + "/" + PlaybackDevices.Count;
            }

            CaptureDevices = MMDeviceManager.CaptureDevices;
            DefaultCaptureDeviceID = MMDeviceManager.DefaultCaptureDeviceId;

            var captureDeviceIndex = CaptureDevices.FindIndex(d => d.ID == DefaultCaptureDeviceID);
            if (captureDeviceIndex != -1)
            {
                var captureDevice = CaptureDevices[captureDeviceIndex];
                DefaultCaptureDeviceName = captureDevice == null ? "Unknown" : TrimName(captureDevice.DeviceInterfaceFriendlyName);
                NavigationStates.CAPTURE_DEVICE.SelectionLabel = DefaultCaptureDeviceName + " " + (captureDeviceIndex + 1) + "/" + CaptureDevices.Count;
            }
        }

        public static string TrimName(string name)
        {
            name = Regex.Replace(name, @"^\d+-\s", "");
            return name;
        }

    }
}
