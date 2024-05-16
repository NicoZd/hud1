using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hud1.Views
{
    /// <summary>
    /// Interaction logic for ScrollPanel.xaml
    /// </summary>
    public partial class ScrollPanel : UserControl
    {
        public static List<ScrollPanel> Instances = new();

        private double _storedPosition;
        public ScrollPanel()
        {
            InitializeComponent();
            Instances.Add(this);
            Console.WriteLine("ScrollPanel Count {0}", Instances.Count);
        }

        internal void SaveScrollPosition()
        {
            var c = (ScrollViewer)GetVisualChild(0);
            _storedPosition = c.VerticalOffset;

        }

        internal void RestoreScrollPosition()
        {
            var c = (ScrollViewer)GetVisualChild(0);
            c.ScrollToVerticalOffset(_storedPosition);
        }

    }
}
