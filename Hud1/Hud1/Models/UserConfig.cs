﻿using Hud1.Helpers;
using Hud1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hud1.Models
{
    public class UserConfig
    {
        public static readonly UserConfig Current = new();

        public UserConfig()
        {
            someInt = 1;
            someString = "two";

            HudPosition = "0:Right";
            GammaIndex = 4;
        }


        public int someInt { get; set; }

        public string someString { get; set; }

        public int GammaIndex { get; set; }

        public string HudPosition { get; set; }
    }
}
