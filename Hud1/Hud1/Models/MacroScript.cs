using Hud1.Helpers;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.System;
using static Hud1.Helpers.WindowsAPI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Hud1.Models;

internal class MacroScript
{
    public bool Running = true;

    private readonly Macro macro;
    private readonly MacroInstructionLimiter debugger;
    private readonly Script script;

    private readonly Queue<SystemEvent> systemEvents = [];

    internal class VKDynamicObject : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            var enumExists = Enum.TryParse(binder.Name, out VirtualKey key);
            result = key;
            return enumExists;
        }
    }
    internal MacroScript(Macro macro)
    {
        this.macro = macro;

        debugger = new MacroInstructionLimiter();
        script = new Script(CoreModules.None | CoreModules.GlobalConsts);

        script.AttachDebugger(debugger);

        //dynamic vk = new VKDynamicObject();
        //Debug.Print($"{vk.W}");
        //S.Globals["MyFlags"] = typeof(MyFlags);
        UserData.RegisterType<VirtualKey>();
        script.Globals.Set("VK", UserData.CreateStatic<VirtualKey>());

        //script.Globals["VK"] = typeof(VirtualKey);

        script.Globals["Label"] = this.macro.Label;
        script.Globals["Description"] = this.macro.Description;

        script.Globals["Running"] = true;

        script.Globals["OnMouseDown"] = (int button) => { };
        script.Globals["OnMouseUp"] = (int button) => { };

        script.Globals["OnKeyDown"] = (int code) => { };
        script.Globals["OnKeyUp"] = (int code) => { };

        script.Globals["Setup"] = () => { };
        script.Globals["Run"] = () => { };
        script.Globals["Cleanup"] = () => { };

        script.Globals["Stop"] = () =>
        {
            macro.Stop();
        };

        script.Globals["Sleep"] = (int a) =>
        {
            Thread.Sleep(a);
            DequeueEvents();
        };

        script.Globals["Millis"] = () =>
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        };


        var debouncedLog = "";
        var lastUpdateTimeMs = (long)0;

        void update()
        {
            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                if (debouncedLog != null)
                {
                    this.macro.Log = debouncedLog;
                    debouncedLog = null;
                }
            }));
            lastUpdateTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        var delay = 50;
        System.Timers.Timer timer = new(TimeSpan.FromMilliseconds(delay));
        timer.Elapsed += (sender, e) => update();
        timer.AutoReset = false;

        script.Globals["Print"] = (string a) =>
        {
            debouncedLog = a;
            if (!timer.Enabled)
            {
                var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var dt = now - lastUpdateTimeMs;
                if (dt > delay)
                {
                    update();
                }
                timer.Start();
            }

        };

        script.Globals["MouseDown"] = () =>
        {
            MouseService.MouseDown(MouseService.MouseButton.Left);
        };

        script.Globals["MouseUp"] = () =>
        {
            MouseService.MouseUp(MouseService.MouseButton.Left);
        };

        script.Globals["FindWindow"] = (string name) =>
        {
            List<string> result = new List<string>();
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if (pr.MainWindowTitle != "")
                {
                    result.Add(pr.MainWindowTitle.ToLower());
                }
            }

            var title = result.Find(s => s.Contains(name.ToLower()));
            if (title != null)
            {
                return WindowsAPI.FindWindow(null, title);
            }
            return -1;
        };

        script.Globals["KeyDown"] = (int window, VirtualKey key) =>
        {
            int wParam = (int)key;
            uint scanCode = WindowsAPI.MapVirtualKey((uint)wParam, 0);
            Debug.Print($"scancode {scanCode}");

            uint lParam = (0x00000001 | (scanCode << 16));
            int r = WindowsAPI.PostMessage(window, WindowMessage.WM_KEYDOWN, wParam, lParam);
            Debug.Print($"KeyDown: {window} {key} {wParam} {lParam} {r}");
        };

        script.Globals["KeyUp"] = (int window, VirtualKey key) =>
        {
            int wParam = (int)key;
            uint scanCode = WindowsAPI.MapVirtualKey((uint)wParam, 0);
            uint lParam = (0xC0000001 | (scanCode << 16));

            int r = WindowsAPI.PostMessage(window, WindowMessage.WM_KEYUP, wParam, lParam);
            Debug.Print($"KeyUp: {window} {key} {wParam} {lParam} {r}");
        };
    }

    internal void ApplyFile()
    {
        var scriptCode = File.ReadAllText(macro.Path);
        script.DoString(scriptCode);
    }

    internal object GetGlobal(string name)
    {
        return script.Globals[name];
    }

    internal void SetGlobal(string name, object value)
    {
        script.Globals[name] = value;
    }

    internal void OnMouseDown()
    {
        Console.WriteLine("OnMouseDown");
        systemEvents.Enqueue(new SystemEvent());
    }

    internal void Run()
    {
        systemEvents.Clear();
        script.Call(script.Globals["Setup"]);
        Thread.Sleep(25);
        DequeueEvents();
        while (Running)
        {
            script.Call(script.Globals["Run"]);
            DequeueEvents();

            Thread.Sleep(25);
            DequeueEvents();
        };
        script.Call(script.Globals["Cleanup"]);
    }

    private class SystemEvent { }

    private void DequeueEvents()
    {
        while (systemEvents.Count > 0)
        {
            var systemEvent = systemEvents.Dequeue();
            script.Call(script.Globals["OnMouseDown"], 0);
        }
    }

    internal void Stop()
    {
        Console.WriteLine("MacroScript Stop");

        Running = false;
        var dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler((_, _) =>
        {
            Console.WriteLine("MacroScript Marking Abort in Debugger");
            dispatcherTimer.Stop();
            debugger.Abort = true;
        });
        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(3000);
        dispatcherTimer.Start();
    }
}
