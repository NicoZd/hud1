using CoreAudio;
using System.Diagnostics;
using System.Windows;
namespace Hud1.Models
{
    public class Volume
    {
        public float Value { get; set; }
        public bool Muted { get; set; }
        public string StringValue
        {
            get { return "" + Math.Round(Value * 100); }
        }

        public string MuteValue
        {
            get { return Muted ? "True" : "False"; }
        }
    }

    public class MMDeviceManager
    {
        public delegate void DevicesUpdateHandler();
        public event DevicesUpdateHandler? DevicesChanged;

        public delegate void VolumeUpdateHandler();
        public event VolumeUpdateHandler? VolumeChanged;

        public String DefaultPlaybackDeviceId = "";
        public String DefaultCaptureDeviceId = "";

        public List<MMDevice> PlaybackDevices = new List<MMDevice>();
        public List<MMDevice> CaptureDevices = new List<MMDevice>();

        private MMDeviceEnumerator deviceEnumerator;
        private MMNotificationClient client;

        public MMDeviceManager()
        {
            deviceEnumerator = new MMDeviceEnumerator(Guid.NewGuid());
            client = new MMNotificationClient(deviceEnumerator);
            client.DefaultDeviceChanged += (s, e) => UpdateDevices();
            UpdateDevices();
        }
        public void SelectNextDevice()
        {
            var index = PlaybackDevices.FindIndex(d => d.ID == DefaultPlaybackDeviceId);
            if (index != -1)
            {
                var nextIndex = Math.Min(index + 1, PlaybackDevices.Count - 1);

                if (nextIndex != index)
                {
                    index = nextIndex;
                    PlaybackDevices[index].Selected = true;
                }
            }
        }

        public void SelectPrevDevice()
        {
            var index = PlaybackDevices.FindIndex(d => d.ID == DefaultPlaybackDeviceId);
            if (index != -1)
            {
                var nextIndex = Math.Max(index - 1, 0);
                if (nextIndex != index)
                {
                    index = nextIndex;
                    PlaybackDevices[index].Selected = true;
                }
            }
        }

        public void SetVolume(float volume)
        {
            var device = PlaybackDevices.Find(d => d.ID == DefaultPlaybackDeviceId);
            if (device != null)
            {
                if (device.AudioEndpointVolume != null)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Max(0, Math.Min(1, volume));
                }
                VolumeChanged?.Invoke();
            }
        }

        public Volume GetVolume()
        {
            var device = PlaybackDevices.Find(d => d.ID == DefaultPlaybackDeviceId);
            if (device != null && device.AudioEndpointVolume != null)
            {
                return new Volume { Value = device.AudioEndpointVolume.MasterVolumeLevelScalar, Muted = device.AudioEndpointVolume.Mute };
            }
            else
            {
                return new Volume { Muted = false, Value = 0.5f };
            }
        }

        public void OnVolumeNotification(AudioVolumeNotificationData data)
        {
            Debug.Print("OnVolumeNotification: {0} {1}", data.MasterVolume, data.Muted);
            //GetVolume();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                VolumeChanged?.Invoke();
            }));
        }

        private void UpdateDevices()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var device = PlaybackDevices.Find(d => d.ID == DefaultPlaybackDeviceId);
                if (device != null && device.AudioEndpointVolume != null)
                {
                    device.AudioEndpointVolume.OnVolumeNotification -= OnVolumeNotification;
                }

                DefaultPlaybackDeviceId = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).ID;
                DefaultCaptureDeviceId = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications).ID;

                PlaybackDevices = new List<MMDevice>(deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active));
                CaptureDevices = new List<MMDevice>(deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active));

                device = PlaybackDevices.Find(d => d.ID == DefaultPlaybackDeviceId);
                Debug.Print("UpdateDevices: {0} {1}", device?.AudioEndpointVolume, device?.DeviceFriendlyName);
                if (device != null && device.AudioEndpointVolume != null)
                {
                    device.AudioEndpointVolume.OnVolumeNotification += OnVolumeNotification;
                }

                VolumeChanged?.Invoke();
                DevicesChanged?.Invoke();
                //LogDevices();
            }));


        }

        private void LogDevices()
        {
            Debug.Print("LogDevices: =================================================== ");
            Debug.Print("defaultPlaybackDeviceID: {0}", DefaultPlaybackDeviceId);
            foreach (var device in PlaybackDevices)
            {
                Debug.Print("\t" + device.DeviceFriendlyName);
            }
            Debug.Print("defaultCaptureDeviceID: {0}", DefaultPlaybackDeviceId);
            foreach (var device in CaptureDevices)
            {
                Debug.Print("\t" + device.DeviceFriendlyName);
            }
        }
    }
}