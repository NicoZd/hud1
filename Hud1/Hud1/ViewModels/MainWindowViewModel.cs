using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public static MainWindowViewModel? Instance;

        [ObservableProperty]
        public Boolean _active = true;

        [ObservableProperty]
        public Visibility _hudVisibility = Visibility.Visible;

        public MainWindow? Window;
        internal nint Hwnd;

        public MainWindowViewModel()
        {
            Instance = this;
        }

        public void ActivateWindow()
        {
            var threadId1 = WindowsAPI.GetWindowThreadProcessId(WindowsAPI.GetForegroundWindow(), IntPtr.Zero);
            var threadId2 = WindowsAPI.GetWindowThreadProcessId(Hwnd, IntPtr.Zero);

            if (threadId1 != threadId2)
            {
                WindowsAPI.AttachThreadInput(threadId1, threadId2, true);
                WindowsAPI.SetForegroundWindow(Hwnd);
                WindowsAPI.AttachThreadInput(threadId1, threadId2, false);
            }
            else
            {
                WindowsAPI.SetForegroundWindow(Hwnd);
            }
        }


        internal void Activate()
        {
            ActivateWindow();
        }

        internal void HandleKeyActivator()
        {
            Debug.Print("HandleKeyActivator");

            if (Active)
            {
                HudVisibility = Visibility.Collapsed;
                Active = false;
            }
            else
            {
                HudVisibility = Visibility.Visible;
                Active = true;

            }
        }
    }
}
