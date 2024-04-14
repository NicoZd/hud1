using Hud1.Controls;
using Hud1.Model;
using Stateless;
using Stateless.Graph;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Hud1
{
    public partial class MainWindow : Window
    {
        WindowModel windowModel = new WindowModel();

        StateMachine<string, string> nav;

        KeyboardListener listener;

        nint hwnd;

        public MainWindow()
        {
            windowModel.PropertyChanged += WindowProp;
            this.DataContext = windowModel;

            nav = new StateMachine<string, string>("center");

            nav.Configure("all")
                .Permit("reset", "center");

            nav.Configure("center")
                .SubstateOf("all")
                .Permit("left", "left-a");

            nav.Configure("left-panel")
                .SubstateOf("all")
                .OnEntry(() => { Debug.Print("enter left panel"); })
                .OnExit(() => { Debug.Print("exit left panel"); });


            nav.Configure("left-a")
                .SubstateOf("left-panel")
                .Permit("right", "center")
                .Permit("down", "left-b");

            nav.Configure("left-b")
                .SubstateOf("left-panel")
                .Permit("right", "center")
                .Permit("up", "left-a");


        }

        private void WindowProp(object sender, PropertyChangedEventArgs e)
        {
            if (windowModel.active)
            {
                Debug.Print("Remove");
                //WindowsServices.RemoveWindowExTransparent(hwnd);
            }
            else
            {
                Debug.Print("Add");
                //WindowsServices.SetWindowExTransparent(hwnd);
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
            //windowModel.active = false;
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

            var container = this.FindName("PART_Conti") as StackPanel;


            var y = new CustomControl2();
            y.Label = "Realtek";            
            container.Children.Add(y);

            y = new CustomControl2();
            y.Label = "Pro X";
            container.Children.Add(y);

            Debug.Print("XX {0}", container);
        }

        private void ListenerOnKeyPressed(object sender, KeyEventArgs e)
        {
            // TYPE YOUR CODE HERE
            Debug.WriteLine("xxx {0}", e.Key);

            if (e.Key == Key.F2)
            {
                windowModel.active = !windowModel.active;

                nav.Fire("reset");
                Debug.Print("nav: {0}", nav.State);

                if (windowModel.active)
                {
                    this.windowModel.active2 = false;
                    this.windowModel.active1 = true;
                    //this.Activate();

                    //TraversalRequest request = new TraversalRequest(FocusNavigationDirection.First);
                    //request.Wrapped = true;
                    //var x = this.MoveFocus(request);
                    //this.Focus();
                    //Debug.Print("X {0}", x);
                }
            }

            if (e.Key == Key.Up)
            {
                // this.Focus();
                //TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Up);
                //request.Wrapped = true;
                //this.MoveFocus(request);
                this.windowModel.active2 = false;
                this.windowModel.active1 = true;

                if (nav.CanFire("up"))
                {
                    nav.Fire("up");
                    Debug.Print("nav: {0}", nav.State);
                }
            }
            if (e.Key == Key.Down)
            {
                // this.Focus();
                //TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Down);
                //request.Wrapped = true;
                //var x = this.MoveFocus(request);
                //Debug.Print("X {0}", x);
                this.windowModel.active1 = false;
                this.windowModel.active2 = true;

                if (nav.CanFire("down"))
                {
                    nav.Fire("down");
                    Debug.Print("nav: {0}", nav.State);
                }

            }
            if (e.Key == Key.Left)
            {
                if (nav.CanFire("left"))
                {
                    nav.Fire("left");
                    Debug.Print("nav: {0}, {1}", nav.State, nav.IsInState("left-panel"));
                }

                // this.Focus();
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Left);
                request.Wrapped = true;
                this.MoveFocus(request);
            }
            if (e.Key == Key.Right)
            {
                if (nav.CanFire("right"))
                {
                    nav.Fire("right");
                    Debug.Print("nav: {0}", nav.State);
                }

                // this.Focus();
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Right);
                request.Wrapped = true;
                this.MoveFocus(request);
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