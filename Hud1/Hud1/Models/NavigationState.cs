using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Hud1.Models;

internal partial class Option : ObservableObject
{
    [ObservableProperty]
    private object _value = "";

    [ObservableProperty]
    private Image? _image = null;

    [ObservableProperty]
    internal bool _selected = false;

    internal Option(object value)
    {
        Value = value;
    }
}

internal class ToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "" + value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "";
    }
}

internal class DisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return "Display " + (intValue + 1);
        }
        else if (value is string stringValue)
        {
            var elems = stringValue.Split(':');
            if (elems.Length == 2)
            {
                var display = int.Parse(elems[0]) + 1;
                return "Display " + display + ", " + elems[1];
            }

        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "";
    }
}

internal class GammaConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int intValue ? "" + NightvisionViewModel.Gammas[intValue] : (object)"";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "";
    }
}


internal class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is double doubleValue ? "" + (Math.Round(doubleValue * 10) / 10) : (object)"";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "";
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
    private object value = "";

    [ObservableProperty]
    private IValueConverter valueConverter = new ToStringConverter();

    [ObservableProperty]
    private string selectionLeftLabel = "⏴";

    [ObservableProperty]
    private string selectionRightLabel = "⏵";

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
    private void Toggle()
    {
        HudViewModel.Instance.SelectNavigationState(this);
        if ((bool)Value)
            ExecuteLeft();
        else
            ExecuteRight();
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
        Value = Options[nextIndex].Value;
    }

    internal void SelectOption()
    {
        var currentIndex = 0;
        try
        {
            currentIndex = Options.Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.Value.Equals(Value)).Index;
        }
        catch (Exception) { };
        Options[currentIndex].Selected = true;
        Value = Options[currentIndex].Value;
    }

    [RelayCommand]
    private void OptionClick(Option option)
    {
        Debug.Print($"OptionClick {option}");
        HudViewModel.Instance.SelectNavigationState(this);
        foreach (var item in Options)
        {
            if (item == option)
            {
                item.Selected = true;
                Value = item.Value;
            }
            else
            {
                item.Selected = false;
            }
        }
    }

    internal void BooleanLeft()
    {
        Value = false;
    }

    internal void BooleanRight()
    {
        Value = true;
    }
}
