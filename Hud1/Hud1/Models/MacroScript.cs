using Hud1.Helpers;
using MoonSharp.Interpreter;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Windows.System;


namespace Hud1.Models;

internal class SystemEvent
{
    internal SystemEventType EventType;
    internal int Button;
    internal KeyEvent? KeyEvent;
    internal POINT? Point;

    internal SystemEvent(SystemEventType eventType, int button, KeyEvent? keyEvent, POINT? point)
    {
        EventType = eventType;
        Button = button;
        KeyEvent = keyEvent;
        Point = point;
    }
}

internal enum SystemEventType
{
    MouseDown = 0,
    MouseUp = 1,
    KeyDown = 2,
    KeyUp = 3
}

internal class LogMessage
{
    internal String Message;
    internal LogMessage(String message)
    {
        Message = message;
    }
}

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
            var name = binder.Name;
            var enumExists = Enum.TryParse(binder.Name, out VirtualKey key);
            result = key;
            return enumExists;
        }
    }
    internal MacroScript(Macro macro)
    {
        this.macro = macro;

        debugger = new MacroInstructionLimiter();
        script = new Script(CoreModules.None | CoreModules.GlobalConsts | CoreModules.Math);

        UserData.RegisterType<POINT>();
        UserData.RegisterType<VirtualKey>();
        UserData.RegisterType<KeyEvent>();
        script.Globals.Set("VK", UserData.CreateStatic<VirtualKey>());

        script.Globals["NameOf"] = (object x) => { return "" + x; };

        script.Globals["Label"] = this.macro.Label;
        script.Globals["Description"] = this.macro.Description;

        script.Globals["Running"] = true;

        script.Globals["OnMouseUp"] = (int button) => { };
        script.Globals["OnMouseDown"] = (int button) => { };

        script.Globals["OnKeyDown"] = (KeyEvent keyEvent) => { };
        script.Globals["OnKeyUp"] = (KeyEvent keyEvent) => { };

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

        LogMessage? debouncedLog = null;
        var lastUpdateTimeMs = (long)0;

        void update()
        {
            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                if (debouncedLog != null)
                {
                    this.macro.Log = debouncedLog.Message ?? "";
                    debouncedLog = null;
                }
            }));
            lastUpdateTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        var delay = 50;
        System.Timers.Timer timer = new(TimeSpan.FromMilliseconds(delay));
        timer.Elapsed += (sender, e) => update();
        timer.AutoReset = false;

        script.Globals["Print"] = (string message) =>
        {
            debouncedLog = new LogMessage(message);
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

        script.Globals["MouseMove"] = (int x, int y) =>
        {
            int MOUSEEVENTF_MOVE = 0x0001;
            int MOUSEEVENTF_ABSOLUTE = 0x8000;

            var p = Monitors.Primary;

            int absoluteX = (x * 65535) / (int)p.Bounds.Width;
            int absoluteY = (y * 65535) / (int)p.Bounds.Height;

            WindowsAPI.mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, absoluteX, absoluteY, 0, 0);
        };

        script.Globals["FindWindow"] = (string name) =>
        {
            List<string> result = [];
            var processRunning = Process.GetProcesses();
            foreach (var pr in processRunning)
            {
                if (pr.MainWindowTitle != "")
                {
                    result.Add(pr.MainWindowTitle.ToLower());
                }
            }

            var title = result.Find(s => s.Contains(name.ToLower()));
            return title != null ? WindowsAPI.FindWindow(null, title) : -1;
        };

        script.Globals["KeyDown"] = (VirtualKey key) =>
        {
            var wParam = (int)key;
            var scanCode = WindowsAPI.MapVirtualKey((uint)wParam, 0);
            Debug.Print($"scancode {scanCode}");

            WindowsAPI.keybd_event((byte)key, (byte)scanCode, 0, 0);

            //uint lParam = (0x00000001 | (scanCode << 16));
            //int r = WindowsAPI.PostMessage(window, WindowMessage.WM_KEYDOWN, wParam, lParam);
            // Debug.Print($"KeyDown: {window} {key} {wParam} {lParam} {r}");
        };

        script.Globals["KeyUp"] = (VirtualKey key) =>
        {
            var wParam = (int)key;
            var scanCode = WindowsAPI.MapVirtualKey((uint)wParam, 0);

            byte KEYEVENTF_KEYUP = 0x0002;
            WindowsAPI.keybd_event((byte)key, (byte)scanCode, KEYEVENTF_KEYUP, 0);

            //uint lParam = (0xC0000001 | (scanCode << 16));
            //int r = WindowsAPI.PostMessage(window, WindowMessage.WM_KEYUP, wParam, lParam);
            //Debug.Print($"KeyUp: {window} {key} {wParam} {lParam} {r}");
        };
    }

    internal void ApplyFile()
    {
        var scriptCode = File.ReadAllText(macro.Path);
        script.AttachDebugger(debugger);
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

    internal void OnMouseDown(int button, POINT point)
    {
        Console.WriteLine($"OnMouseDown button: {button}");
        systemEvents.Enqueue(new SystemEvent(SystemEventType.MouseDown, button, null, point));
    }

    internal void OnMouseUp(int button, POINT point)
    {
        Console.WriteLine($"OnMouseUp button: {button}");
        systemEvents.Enqueue(new SystemEvent(SystemEventType.MouseUp, button, null, point));
    }

    internal void OnKeyDown(KeyEvent keyEvent)
    {
        Console.WriteLine($"OnKeyDown key: {keyEvent}");
        systemEvents.Enqueue(new SystemEvent(SystemEventType.KeyDown, 0, keyEvent, null));
    }

    internal void OnKeyUp(KeyEvent keyEvent)
    {
        Console.WriteLine($"OnKeyUp key: {keyEvent}");
        systemEvents.Enqueue(new SystemEvent(SystemEventType.KeyUp, 0, keyEvent, null));
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

    private void DequeueEvents()
    {
        while (systemEvents.Count > 0)
        {
            var systemEvent = systemEvents.Dequeue();

            switch (systemEvent.EventType)
            {
                case SystemEventType.MouseUp:
                    {
                        script.Call(script.Globals["OnMouseUp"], systemEvent.Button, systemEvent.Point);
                        break;
                    }
                case SystemEventType.MouseDown:
                    {
                        script.Call(script.Globals["OnMouseDown"], systemEvent.Button, systemEvent.Point);
                        break;
                    }
                case SystemEventType.KeyDown:
                    {
                        script.Call(script.Globals["OnKeyDown"], systemEvent.KeyEvent);
                        break;
                    }
                case SystemEventType.KeyUp:
                    {
                        script.Call(script.Globals["OnKeyUp"], systemEvent.KeyEvent);
                        break;
                    }
            }
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
