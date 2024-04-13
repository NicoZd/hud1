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
        public Boolean active
        {
            get { return _active; }
            set { _active = value; OnPropertyChanged(); }
        }
        public Boolean _active1 = false;
        public Boolean active1
        {
            get { return _active1; }
            set { _active1 = value; OnPropertyChanged(); }
        }
        public Boolean _active2 = false;
        public Boolean active2
        {
            get { return _active2; }
            set { _active2 = value; OnPropertyChanged(); }
        }
    }
}
