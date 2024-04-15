using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hud1.Model
{
    class WindowModel: ObservableObject
    {
        public Boolean _active = false;
        public Boolean Active
        {
            get { return _active; }
            set { _active = value; OnPropertyChanged(); }
        }

        public String _state = "State";
        public String State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }

        //panel-left, panel-right, panel-top, panel-bottom
        public String _panel = "Panel";
        public String Panel
        {
            get { return _panel; }
            set { _panel = value; OnPropertyChanged(); }
        }
    }
}
