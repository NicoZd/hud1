﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Windows.UI;

namespace Hud1.Helpers
{
    class TopMostHelper
    {
        public static readonly TopMostHelper Instance = new TopMostHelper();

        public List<nint> TopWindows = [];

        private TopMostHelper()
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((_, _) =>
            {
                Run();
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void Run()
        {
            //Debug.Print($"TopMostHelper Run {TopWindows.Count}");
            TopWindows.ForEach(hwnd =>
            {
                //Debug.Print($"MakeTopMost {hwnd}");
                WindowsAPI.SetWindowPos(hwnd, WindowsAPI.HWND_TOP, 0, 0, 0, 0, WindowsAPI.SetWindowPosFlags.SWP_NOMOVE | WindowsAPI.SetWindowPosFlags.SWP_NOSIZE | WindowsAPI.SetWindowPosFlags.SWP_NOACTIVATE);
            });
        }

    }
}
