using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;


namespace Hud1.Models;

internal partial class Option : ObservableObject
{
    [ObservableProperty]
    private string _value = "";

    [ObservableProperty]
    private Image? _image = null;

    [ObservableProperty]
    internal bool _selected = false;

    internal Option(string value)
    {
        Value = value;
    }
}

internal partial class NavigationState : ObservableObject
{
    internal static bool Repeat = false;
    internal bool AllowRepeat { get; set; } = false;


    [ObservableProperty]
    internal bool selectRight;
    private int _selectRightCounter = 0;

    [ObservableProperty]
    internal bool selectLeft;
    private int _selectLeftCounter = 0;

    [ObservableProperty]
    private bool selected;

    [ObservableProperty]
    private Visibility _visibility;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _label = "";

    [ObservableProperty]
    private string _hint = "";

    [ObservableProperty]
    private string selectionLabel = "";

    [ObservableProperty]
    private bool selectionBoolean = true;

    [ObservableProperty]
    private string selectionLeftLabel = "<";

    [ObservableProperty]
    private string selectionRightLabel = ">";

    internal Action? LeftAction { get; set; }

    internal Action? RightAction { get; set; }

    [ObservableProperty]
    private ObservableCollection<Option> _options = [];

    [ObservableProperty]
    private int _spacing = 4;

    internal NavigationState([CallerMemberName] string Name = "")
    {
        this.Name = Name;
    }

    public override string? ToString()
    {
        return Name;
    }

    [RelayCommand]
    private void Left()
    {
        HudViewModel.Instance.SelectNavigationState(this);
        ExecuteLeft();
    }

    [RelayCommand]
    private void Right()
    {
        HudViewModel.Instance.SelectNavigationState(this);
        ExecuteRight();
    }

    [RelayCommand]
    private void Click()
    {
        HudViewModel.Instance.SelectNavigationState(this);
    }

    internal async void ExecuteLeft()
    {
        Debug.Print("ExecuteLeft");
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

    internal async void ExecuteRight()
    {
        Debug.Print("ExecuteRight");
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

    internal void OptionLeft()
    {
        OptionSelect(-1);
    }

    internal void OptionRight()
    {
        OptionSelect(1);
    }

    internal void OptionSelect(int dir)
    {
        var currentIndex = -1;

        try
        {
            currentIndex = Options.Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.Selected).Index;
        }
        catch (Exception) { };

        if (currentIndex == -1)
        {
            currentIndex = 0;
            dir = 0;
        }

        var nextIndex = Math.Min(Math.Max(currentIndex + dir, 0), Options.Count - 1);
        Options[currentIndex].Selected = false;
        Options[nextIndex].Selected = true;
        SelectionLabel = Options[nextIndex].Value;
    }

    internal void SelectOption()
    {
        var currentIndex = 0;
        try
        {
            currentIndex = Options.Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.Value == SelectionLabel).Index;
        }
        catch (Exception) { };
        Options[currentIndex].Selected = true;
    }

    internal void BooleanLeft()
    {
        SelectionBoolean = false;
    }

    internal void BooleanRight()
    {
        SelectionBoolean = true;
    }
}
