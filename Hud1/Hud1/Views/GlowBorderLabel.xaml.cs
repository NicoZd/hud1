using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hud1.Views;

[INotifyPropertyChanged]
public partial class GlowBorderLabel : UserControl
{
    private static readonly DependencyProperty LabelProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, string>("Label", "");

    private static readonly DependencyProperty HighlightedProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, bool>("Highlighted", false);

    private static readonly DependencyProperty SelectedProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, bool>("Selected", false);

    private static readonly DependencyProperty PressedProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, bool>("Pressed", false);

    private static readonly DependencyProperty EnabledProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, bool>("Enabled", true);

    private static readonly DependencyProperty NavigationStateProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, NavigationState>("NavigationState", null);

    private static readonly DependencyProperty ClickProperty =
        BindingHelper.CreateProperty<GlowBorderLabel, ICommand>("Click", null);

    public string Label
    {
        set => SetValue(LabelProperty, value);
        get => (string)GetValue(LabelProperty);
    }

    public bool Highlighted
    {
        set => SetValue(HighlightedProperty, value);
        get => (bool)GetValue(HighlightedProperty);
    }
    public bool Selected
    {
        set => SetValue(SelectedProperty, value);
        get => (bool)GetValue(SelectedProperty);
    }
    public bool Pressed
    {
        set => SetValue(PressedProperty, value);
        get => (bool)GetValue(PressedProperty);
    }
    public bool Enabled
    {
        set => SetValue(EnabledProperty, value);
        get => (bool)GetValue(EnabledProperty);
    }
    public ICommand Click
    {
        set => SetValue(ClickProperty, value);
        get => (ICommand)GetValue(ClickProperty);
    }

    public NavigationState NavigationState
    {
        set => SetValue(NavigationStateProperty, value);
        get => (NavigationState)GetValue(NavigationStateProperty);
    }

    public GlowBorderLabel()
    {
        InitializeComponent();
    }
}
