using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(OnTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            switch ("blue")
            {
                case "green":
                    Resources["ColorSuperBright"] = (Color)ColorConverter.ConvertFromString("#bbffbb");
                    Resources["ColorBright"] = (Color)ColorConverter.ConvertFromString("#99cc99");
                    break;
                case "blue":
                    Resources["ColorSuperBright"] = (Color)ColorConverter.ConvertFromString("#99ccff");
                    Resources["ColorBright"] = (Color)ColorConverter.ConvertFromString("#336699");
                    break;
            }
        }
    }

}
