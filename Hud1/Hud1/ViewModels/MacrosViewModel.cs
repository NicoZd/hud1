using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Models;
using MoonSharp.Interpreter;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Hud1.ViewModels
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
                string scriptCode = File.ReadAllText(_path);
                _script = new Script(CoreModules.None);
                _script.Globals["label"] = Label;
                _script.Globals["description"] = Description;
                _script.DoString(scriptCode);
                Label = (string)_script.Globals["label"];
                Description = (string)_script.Globals["description"];
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
            if (Running && _script != null)
            {
                RightLabel = "Stopping";
                _script.Globals["running"] = false;
                return;
            }
            Error = "";
            RightLabel = "Stop ▶";
            Running = true;

            string scriptCode = File.ReadAllText(_path);

            _script = new Script(CoreModules.None);
            _script.Globals["sleep"] = (int a) =>
            {
                //Debug.Print("SLEEP {0}", a);
                Thread.Sleep(a);
            };


            _script.Globals["print"] = (string a) =>
            {
                //Debug.Print("print {0}", a);
                Log = a;
            };

            _script.Globals["running"] = true;

            ThreadPool.QueueUserWorkItem((_) =>
            {
                try
                {
                    _script.DoString(scriptCode);
                    _script.Call(_script.Globals["setup"]);
                    while ((bool)_script.Globals["running"])
                    {
                        _script.Call(_script.Globals["run"]);
                    };
                    _script.Call(_script.Globals["cleanup"]);
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

    public partial class MacrosViewModel : ObservableObject
    {
        public ObservableCollection<Macro> Macros { get; set; } = new();

        public Stateless.StateMachine<NavigationState, NavigationTrigger>? Navigation;

        [ObservableProperty]
        public int selectedIndex = -1;

        [ObservableProperty]
        public bool selected = false;

        private FileSystemWatcher _watcher;

        private String _path = "";

        public MacrosViewModel()
        {
            Macros = new ObservableCollection<Macro>();

            _path = Path.Combine(Directory.GetCurrentDirectory(), "Macros");
            Debug.Print("Macros Path {0}", _path);

            _watcher = new FileSystemWatcher(_path);
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
            _watcher.Changed += OnDirectoryChanged;
            _watcher.Created += OnDirectoryChanged;
            _watcher.Deleted += OnDirectoryChanged;
            _watcher.Renamed += OnDirectoryChanged;
            _watcher.EnableRaisingEvents = true;

            UpdateFiles();
        }

        private void OnDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UpdateFiles();
            }));
        }

        private void UpdateFiles()
        {
            Debug.Print("UpdateFiles");
            foreach (Macro macro in Macros)
                macro.Running = false;

            string[] fileEntries = Directory.GetFiles(_path);
            var temp = new ObservableCollection<Macro>();
            foreach (string fileEntry in fileEntries)
            {
                temp.Add(new Macro(fileEntry));
            }

            Macros.Clear();
            foreach (Macro m in temp.OrderBy(m => m.Label))
            {
                Macros.Add(m);
            }

            var lastIndex = SelectedIndex;
            SelectedIndex = -1;
            SelectedIndex = lastIndex;
        }

        partial void OnSelectedIndexChanged(int value)
        {
            Debug.Print("OnSelectedIndexChanged", value);

            for (var i = 0; i < Macros.Count; i++)
            {
                var macro = Macros[i];
                macro.Selected = i == value;
                if (i == value)
                {
                    NavigationStates.MACROS.Hint = macro.Description;
                }
            }
        }

        public void OnEntry()
        {
            Debug.Print("OnEntry");
            Selected = true;

            if (Macros.Count == 0)
            {
                Navigation!.Fire(NavigationTriggers.RETURN);
            }
        }

        public void OnExit()
        {
            Debug.Print("OnExit");
            Selected = false;
            SelectedIndex = -1;
        }

        public void OnEntryFromTop()
        {
            Debug.Print("OnEntryFromTop");
            SelectedIndex = 0;
        }

        public void OnEntryFromBottom()
        {
            Debug.Print("OnEntryFromBottom");
            SelectedIndex = Macros.Count - 1;
        }

        public void OnUp()
        {
            Debug.Print("OnUp {0}", SelectedIndex);
            if (SelectedIndex <= 0)
            {
                Navigation?.Fire(NavigationTriggers.RETURN);
                return;
            }
            SelectedIndex--;
        }
        public void OnDown()
        {
            Debug.Print("OnDown {0}", SelectedIndex);
            if (SelectedIndex >= Macros.Count - 1)
            {
                Navigation?.Fire(NavigationTriggers.RETURN);
                return;
            }
            SelectedIndex++;
        }

        public void OnLeft()
        {
            Debug.Print("OnLeft");
            Macros[SelectedIndex].OnLeft();
        }
        public void OnRight()
        {
            Debug.Print("OnRight");
            Macros[SelectedIndex].OnRight();
        }

    }

}
