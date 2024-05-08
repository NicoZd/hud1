using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using MoonSharp.Interpreter;
using System.Diagnostics;

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

        [ObservableProperty]
        private string _path = "";

        private MacroScript? _macroScript;

        public Macro(String path)
        {
            Path = path;

            Label = System.IO.Path.GetFileName(Path);
            Description = "";
            RightLabel = "Start >";

            FetchProgramMetaData();
        }

        private void FetchProgramMetaData()
        {
            try
            {
                var script = new MacroScript(this);
                Label = (string)script.GetGlobal("Label");
                Description = (string)script.GetGlobal("Description");
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

        internal void OnLeft()
        {
        }

        internal void OnRight()
        {
            if (Running && _macroScript != null)
            {
                RightLabel = "Stopping";
                _macroScript.SetGlobal("Running", false);
                return;
            }

            Error = "";
            Running = true;
            RightLabel = "Stop >";

            ThreadPool.QueueUserWorkItem((_) =>
            {
                try
                {
                    _macroScript = new MacroScript(this);
                    using (var hooks = new ScriptHooks(_macroScript))
                    {
                        _macroScript.Run();
                    }
                }
                catch (InterpreterException ex)
                {
                    Debug.Print("ERROR {0}", ex.DecoratedMessage);

                    if (ex.DecoratedMessage != null)
                        Error = ex.DecoratedMessage;
                    else
                        Error = "ERROR: " + ex.Message;
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
                Running = false;
                RightLabel = "Start >";
            });
        }

        public class ScriptHooks : IDisposable
        {
            private MacroScript _macroScript;

            public ScriptHooks(MacroScript macroScript)
            {
                _macroScript = macroScript;
                GlobalMouseHook.MouseDown += OnMouseDown;
            }

            private void OnMouseDown()
            {
                Debug.Print("OnMouseDown");
                _macroScript.OnMouseDown();
            }

            public void Dispose()
            {
                Debug.Print("MACROHOOK Dispose");
                GlobalMouseHook.MouseDown -= OnMouseDown;
            }
        }
    }
}
