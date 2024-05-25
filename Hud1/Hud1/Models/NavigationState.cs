using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.ViewModels;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;


namespace Hud1.Models;

public partial class Option : ObservableObject
{
    [ObservableProperty]
    private string _value = "";

    [ObservableProperty]
    private Image _image = null;

    [ObservableProperty]
    public bool _selected = false;

    public Option(string value)
    {
        this.Value = value;
    }
}

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

    [ObservableProperty]
    private ObservableCollection<Option> _options = [];

    [ObservableProperty]
    private int _spacing = 4;

    public NavigationState([CallerMemberName] string Name = "")
    {
        this.Name = Name;
    }

    [RelayCommand]
    private void Left()
    {
        NavigationViewModel.SelectNavigationState(this);
        ExecuteLeft();
    }

    [RelayCommand]
    private void Right()
    {
        NavigationViewModel.SelectNavigationState(this);
        ExecuteRight();
    }

    [RelayCommand]
    private void Click()
    {
        NavigationViewModel.SelectNavigationState(this);
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

    public void OptionLeft()
    {
        OptionSelect(-1);
    }

    public void OptionRight()
    {
        OptionSelect(1);
    }

    public void OptionSelect(int dir)
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
