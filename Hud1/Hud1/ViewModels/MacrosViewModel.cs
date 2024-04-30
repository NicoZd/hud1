using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace Hud1.ViewModels
{
    public partial class Macro : ObservableObject
    {
        [ObservableProperty]
        private string _name = "Hellooo!";

        [ObservableProperty]
        private bool _selected = false;

    }

    public partial class MacrosViewModel : ObservableObject
    {
        public ObservableCollection<Macro> Macros { get; set; }

        public Stateless.StateMachine<NavigationState, NavigationTrigger>? Navigation;

        [ObservableProperty]
        public int selectedIndex = -1;

        [ObservableProperty]
        public bool selected = false;

        public MacrosViewModel()
        {
            Macros = new ObservableCollection<Macro>();
            Macros.Add(new Macro());
            Macros.Add(new Macro());
        }

        partial void OnSelectedIndexChanged(int value)
        {
            Debug.Print("OnSelectedIndexChanged", value);

            for (var i = 0; i < Macros.Count; i++)
            {
                var macro = Macros[i];
                macro.Selected = i == value;
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
        }
        public void OnRight()
        {
            Debug.Print("OnRight2");
        }

    }

}
