using Hud1.Model;
using System;
using System.Collections.Generic;
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

namespace Hud1.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomControl2 : UserControl
    {
        CustomControl2Model model = new CustomControl2Model();

        public string Label
        {
            get => (string)PART_Label.Content;
            set => PART_Label.Content = value;
        }

        private bool _selected = false;
        public bool Selected
        {
            get => model.Selected;
            set => model.Selected = value;

        }

        public CustomControl2()
        {
            InitializeComponent();
            this.DataContext = model;
        }

    }
}
