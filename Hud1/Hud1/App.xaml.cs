using System.Windows;

namespace Hud1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(OnTick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //dispatcherTimer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            //Resources["LabelColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffbbff00"));
            //Resources["FontFamily"] = new FontFamily("Cascadia Code");
        }
    }

}
