using Hud1.model;
using Stateless;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Hud1
{
    public partial class MainWindow: Window
    {
        WindowModel windowModel = new WindowModel();
        KeyboardListener listener;

        nint hwnd;
        public MainWindow()
        {
            windowModel.PropertyChanged += WindowProp;
            this.DataContext = windowModel;

            const string on = "On";
            const string off = "Off";
            const char space = ' ';

            // Instantiate a new state machine in the 'off' state
            var onOffSwitch = new StateMachine<string, char>(off);

            // Configure state machine with the Configure method, supplying the state to be configured as a parameter
            onOffSwitch.Configure(off).Permit(space, on);
            onOffSwitch.Configure(on).Permit(space, off);
        }

        private void WindowProp(object sender, PropertyChangedEventArgs e)
        {
            if (windowModel.active)
            {
                Debug.Print("Remove");
                // WindowsServices.RemoveWindowExTransparent(hwnd);
            }
            else
            {
                Debug.Print("Add");
                // WindowsServices.SetWindowExTransparent(hwnd);
            }
        }
       

        private void OnWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnWindowActivated");
            windowModel.active = true;
        }
        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnWindowDeactivated");
            windowModel.active = false;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            hwnd = new WindowInteropHelper(this).Handle;
            Debug.WriteLine("OnSourceInitialized {0}", hwnd);

            WindowsServices.SetWindowExTransparent(hwnd);

            int width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            Debug.WriteLine("Window_Loaded {0} {1}", width, height);

            this.Width = width;
            this.Height = height;
            this.Left = 0;
            this.Top = 0;

            listener = new KeyboardListener();
            listener.KeyboardDownEvent += ListenerOnKeyPressed;
        }

        private void ListenerOnKeyPressed(object sender, KeyEventArgs e)
        {
            // TYPE YOUR CODE HERE
            Debug.WriteLine("xxx {0}", e.Key);

            if (e.Key == Key.F2) {
                windowModel.active = !windowModel.active;
            }
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            listener.KeyboardDownEvent -= ListenerOnKeyPressed;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}