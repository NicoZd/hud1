using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using MoonSharp.Interpreter;
using System.Diagnostics;
using System.IO;

namespace Hud1.Models
{
    public partial class Macro : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private string _description = "";

        [ObservableProperty]
        private string _log = "";

        [ObservableProperty]
        private string _error = "";

        [ObservableProperty]
        private bool _selected = false;

        [ObservableProperty]
        private bool _running = false;

        [ObservableProperty]
        private string _rightLabel = "";

        private string _path = "";

        private Script? _script;

        public Macro(String path)
        {
            _path = path;
            Label = Path.GetFileName(path);
            Description = "";
            RightLabel = "Start ▶";

            try
            {
                var script = CreateScript();
                Label = (string)script.Globals["Label"];
                Description = (string)script.Globals["Description"];
            }
            catch (InterpreterException ex)
            {
                Error = ex.DecoratedMessage;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        Script CreateScript()
        {
            string scriptCode = File.ReadAllText(_path);

            var script = new Script(CoreModules.None);

            script.Globals["Label"] = Label;

            script.Globals["Description"] = Description;

            script.Globals["Running"] = true;

            script.Globals["Sleep"] = (int a) =>
            {
                Thread.Sleep(a);
            };

            script.Globals["Print"] = (string a) =>
            {
                Log = a;
            };

            script.Globals["IsLeftMouseDown"] = () =>
            {
                return GlobalMouseHook.IsLeftMouseDown;
            };

            script.Globals["MouseDown"] = () =>
            {
                MouseService.MouseDown(MouseService.MouseButton.Left);
            };

            script.Globals["MouseUp"] = () =>
            {
                MouseService.MouseUp(MouseService.MouseButton.Left);
            };

            script.DoString(scriptCode);

            return script;
        }

        internal void OnLeft()
        {
        }

        internal void OnRight()
        {
            if (Running && _script != null)
            {
                RightLabel = "Stopping";
                _script.Globals["Running"] = false;
                return;
            }

            Error = "";
            Running = true;
            RightLabel = "Stop ▶";

            ThreadPool.QueueUserWorkItem((_) =>
            {
                try
                {
                    _script = CreateScript();
                    _script.Call(_script.Globals["Setup"]);
                    while ((bool)_script.Globals["Running"])
                    {
                        _script.Call(_script.Globals["Run"]);
                        Thread.Sleep(100);
                    };
                    _script.Call(_script.Globals["Cleanup"]);
                }
                catch (InterpreterException ex)
                {
                    Debug.Print("ERROR {0}", ex.DecoratedMessage);
                    Error = ex.DecoratedMessage;
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
                Running = false;
                RightLabel = "Start ▶";
            });
        }
    }
}
