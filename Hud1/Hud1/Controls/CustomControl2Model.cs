using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hud1.Model;

namespace Hud1.Controls
{
    public class CustomControl2Model: ObservableObject
    {
        public Boolean _selected = false;
        public Boolean Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged(); }
        }

        public String _label = "";
        public String Label
        {
            get { return _label; }
            set { _label = value; OnPropertyChanged(); }
        }
    }
}
