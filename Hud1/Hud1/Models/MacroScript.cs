using Hud1.Helpers;
using MoonSharp.Interpreter;
using System.Diagnostics;
using System.IO;

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
            _script = new Script(CoreModules.None);

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

            _script.Globals["Print"] = (string a) =>
            {
                _macro.Log = a;
                Debug.Print("SCRIPT: {0}", a);
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
                _script.Call(_script.Globals["OnMouseDown"]);
            }
        }

        internal void Run()
        {
            _systemEvents.Clear();
            _script.Call(_script.Globals["Setup"]);
            while ((bool)_script.Globals["Running"])
            {
                _script.Call(_script.Globals["Run"]);
                DequeueEvents();

                Thread.Sleep(50);
                DequeueEvents();
            };
            _script.Call(_script.Globals["Cleanup"]);
        }
    }
}
