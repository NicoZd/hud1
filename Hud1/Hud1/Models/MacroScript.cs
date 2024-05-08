using CoreAudio;
using Hud1.Helpers;
using MoonSharp.Interpreter;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;

namespace Hud1.Models
{
    public class MacroScript
    {
        private Macro _macro;
        private Script _script;

        private Queue<SystemEvent> _systemEvents = [];

        public MacroScript(Macro macro)
        {
            _macro = macro;
            _script = new Script(CoreModules.None | CoreModules.GlobalConsts);

            _script.Globals["Label"] = _macro.Label;
            _script.Globals["Description"] = _macro.Description;

            _script.Globals["Running"] = true;

            _script.Globals["OnMouseDown"] = (int button) => { };
            _script.Globals["OnMouseUp"] = (int button) => { };

            _script.Globals["OnKeyDown"] = (int code) => { };
            _script.Globals["OnKeyUp"] = (int code) => { };

            _script.Globals["Setup"] = () => { };
            _script.Globals["Run"] = () => { };
            _script.Globals["Cleanup"] = () => { };

            _script.Globals["Stop"] = () =>
            {
                _script.Globals["Running"] = false;
            };

            _script.Globals["Sleep"] = (int a) =>
            {
                Thread.Sleep(a);
                DequeueEvents();
            };

            _script.Globals["Millis"] = () =>
            {
                return DateTimeOffset.Now.ToUnixTimeMilliseconds();
            };


            var debouncedLog = "";
            var lastUpdateTimeMs = (long)0;

            var update = () =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (debouncedLog != null)
                    {
                        _macro.Log = debouncedLog;
                        debouncedLog = null;
                    }
                }));
                lastUpdateTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            };

            var delay = 50;
            System.Timers.Timer timer = new(TimeSpan.FromMilliseconds(delay));
            timer.Elapsed += async (sender, e) => update();
            timer.AutoReset = false;

            _script.Globals["Print"] = (string a) =>
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

            _script.Globals["MouseDown"] = () =>
            {
                MouseService.MouseDown(MouseService.MouseButton.Left);
            };

            _script.Globals["MouseUp"] = () =>
            {
                MouseService.MouseUp(MouseService.MouseButton.Left);
            };

            string scriptCode = File.ReadAllText(_macro.Path);
            _script.DoString(scriptCode);
        }

        public object GetGlobal(string name)
        {
            return _script.Globals[name];
        }

        public void SetGlobal(string name, object value)
        {
            _script.Globals[name] = value;
        }

        internal void OnMouseDown()
        {
            Debug.Print("OnMouseDown");
            _systemEvents.Enqueue(new SystemEvent());
        }

        class SystemEvent { }

        internal void DequeueEvents()
        {
            while (_systemEvents.Count > 0)
            {
                var systemEvent = _systemEvents.Dequeue();
                _script.Call(_script.Globals["OnMouseDown"], 0);
            }
        }

        internal void Run()
        {
            _systemEvents.Clear();
            _script.Call(_script.Globals["Setup"]);
            Thread.Sleep(25);
            DequeueEvents();
            while ((bool)_script.Globals["Running"])
            {
                _script.Call(_script.Globals["Run"]);
                DequeueEvents();

                Thread.Sleep(25);
                DequeueEvents();
            };
            _script.Call(_script.Globals["Cleanup"]);
        }
    }
}
