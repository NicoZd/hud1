using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Windows.Controls;

namespace Hud1.Views
{
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
            if (VisualChildrenCount > 0)
            {
                var c = (ScrollViewer)GetVisualChild(0);
                _storedPosition = c.VerticalOffset;
            }
            else
            {
                _storedPosition = 0;
            }
        }

        internal void RestoreScrollPosition()
        {
            if (VisualChildrenCount > 0)
            {
                var c = (ScrollViewer)GetVisualChild(0);
                c.ScrollToVerticalOffset(_storedPosition);
            }
        }

    }
}
