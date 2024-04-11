using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hud1.model
{
    class WindowModel: ObservableObject
    {
        public Boolean _active = false;
        public Boolean active
        {
            get { return _active; }
            set { _active = value; OnPropertyChanged(); }
        }
    }
}
