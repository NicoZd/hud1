using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hud1.Models
{
    public class UserConfig
    {
        public UserConfig()
        {
            someInt = 1;
            someString = "two";
        }

        public int someInt { get; set; }

        public string someString { get; set; }
    }
}
