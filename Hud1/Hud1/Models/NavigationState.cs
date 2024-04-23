using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Hud1.Models
{
    public partial class NavigationState : ObservableObject
    {
        public bool Selected { get; set; }

        [ObservableProperty]
        public bool selectRight;

        public Visibility Visibility { get; set; }

        public string Label { get; set; }

        public Action? ExecuteRightAction { get; set; }

        public NavigationState([CallerMemberName] string label = "")
        {
            this.Label = label;
            Selected = false;
            SelectRight = false;
            Visibility = Visibility.Collapsed;
        }

        public async void ExecuteRight()
        {
            SelectRight = true;
            await Task.Delay(50);
            if (ExecuteRightAction != null)
                try { ExecuteRightAction(); } catch (Exception ex) { Debug.Print("ExecuteRight {0}", ex); }
            await Task.Delay(50);
            SelectRight = false;
        }

        public override string? ToString()
        {
            return Label;
        }
    }
}
