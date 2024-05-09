using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Hud1.ViewModels
{
    public partial class MacrosViewModel : ObservableObject
    {
        public ObservableCollection<Macro> Macros { get; set; } = new();

        public Stateless.StateMachine<NavigationState, NavigationTrigger>? Navigation;

        [ObservableProperty]
        public int selectedIndex = -1;

        [ObservableProperty]
        public bool selected = false;

        private FileSystemWatcher _watcher;

        public String _path = "";

        public MacrosViewModel()
        {
            Macros = new ObservableCollection<Macro>();

            var exeFolder = Path.GetDirectoryName(Process.GetCurrentProcess()!.MainModule!.FileName);
            _path = Path.Combine(exeFolder!, "Macros");

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
                temp.Add(new Macro(fileEntry, this));
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
                Navigation!.Fire(NavigationTriggers.RETURN_UP);
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
                Navigation?.Fire(NavigationTriggers.RETURN_UP);
                return;
            }
            SelectedIndex--;
        }
        public void OnDown()
        {
            Debug.Print("OnDown {0}", SelectedIndex);
            if (SelectedIndex >= Macros.Count - 1)
            {
                Navigation?.Fire(NavigationTriggers.RETURN_DOWN);
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

        internal void SelectMacro(Macro macro)
        {
            HudViewModel.SelectNavigationState(NavigationStates.MACROS);
            var index = Macros.IndexOf(macro);
            if (index >= 0)
            {
                SelectedIndex = index;
            }
        }
    }

}
