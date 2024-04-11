using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hud1
{
    public partial class MainWindow: INotifyPropertyChanged

    {
        public double _ddd;
        public double ddd
        {
            get {
                return _ddd;
            }
            
            set {
                _ddd = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            Debug.WriteLine("OnSourceInitialized {0}", hwnd);
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            int width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            Debug.WriteLine("Window_Loaded {0} {1}", width, height);

            this.Width = width;
            this.Height = height;
            this.Left = 0;
            this.Top = 0;
        }

        private void OnWindowGotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnWindowGotFocus");
        }

        private void OnWindowLostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnWindowLostFocus");
        }
        private void OnWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnWindowActivated");
            ddd = 4;
        }
        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnWindowDeactivated");
            ddd = 1;
        }

    }
}