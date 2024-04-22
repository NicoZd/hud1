using CoreAudio;
using System.Diagnostics;
using System.Windows;
namespace Hud1.Models
{
    public class MMDeviceManager
    {
        public delegate void DevicesUpdateHandler();
        public event DevicesUpdateHandler? DevicesChanged;

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

        public void UpdateDevices()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                DefaultPlaybackDeviceId = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).ID;
                DefaultCaptureDeviceId = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications).ID;

                PlaybackDevices = new List<MMDevice>(deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active));
                CaptureDevices = new List<MMDevice>(deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active));

                DevicesChanged?.Invoke();
                //LogDevices();
            }));
        }

        public void LogDevices()
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