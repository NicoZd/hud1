using Hud1.Helpers;
using Hud1.Models;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views;

public partial class BooleanPanel : UserControl
{
    private static readonly DependencyProperty NavigationStateProperty =
        BindingHelper.CreateProperty<BooleanPanel, NavigationState>("NavigationState", null);

    internal NavigationState NavigationState
    {
        get => (NavigationState)GetValue(NavigationStateProperty);
        set => SetValue(NavigationStateProperty, value);
    }

    public BooleanPanel()
    {
        InitializeComponent();
    }
}
