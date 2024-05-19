using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Services;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;


namespace Hud1.Models
{
    public partial class NavigationState : ObservableObject
    {
        public static bool Repeat = false;
        public bool AllowRepeat { get; set; } = false;


        [ObservableProperty]
        public bool selectRight;
        private int _selectRightCounter = 0;

        [ObservableProperty]
        public bool selectLeft;
        private int _selectLeftCounter = 0;

        [ObservableProperty]
        private bool selected;

        public Visibility Visibility { get; set; }

        public string Name { get; set; }
        public string Label { get; set; } = "";

        [ObservableProperty]
        public string _hint = "";

        [ObservableProperty]
        public string selectionLabel = "";

        [ObservableProperty]
        public bool selectionBoolean = true;

        [ObservableProperty]
        public string selectionLeftLabel = "<";

        [ObservableProperty]
        public string selectionRightLabel = ">";

        public Action? LeftAction { get; set; }

        public Action? RightAction { get; set; }

        public NavigationState([CallerMemberName] string Name = "")
        {
            this.Name = Name;
        }

        [RelayCommand]
        private void Left()
        {
            NavigationService.SelectNavigationState(this);
            ExecuteLeft();
        }

        [RelayCommand]
        private void Right()
        {
            NavigationService.SelectNavigationState(this);
            ExecuteRight();
        }

        [RelayCommand]
        private void Click()
        {
            NavigationService.SelectNavigationState(this);
        }

        public async void ExecuteLeft()
        {
            SelectLeft = true;
            _selectLeftCounter++;
            await Task.Delay(10);
            if (LeftAction != null)
                try { LeftAction(); } catch (Exception ex) { Console.WriteLine("ExecuteLeft {0}", ex); }
            await Task.Delay(100);
            _selectLeftCounter--;
            if (_selectLeftCounter == 0)
                SelectLeft = false;
        }

        public async void ExecuteRight()
        {
            SelectRight = true;
            _selectRightCounter++;
            await Task.Delay(10);
            if (RightAction != null)
                try { RightAction(); } catch (Exception ex) { Console.WriteLine("ExecuteRight {0}", ex); }
            await Task.Delay(100);
            _selectRightCounter--;
            if (_selectRightCounter == 0)
                SelectRight = false;
        }

        public override string? ToString()
        {
            return Name;
        }
    }
}
