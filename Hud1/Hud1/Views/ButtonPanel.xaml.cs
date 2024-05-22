using Hud1.Helpers;
using Hud1.Models;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views;

public partial class ButtonPanel : UserControl
{
    private static readonly DependencyProperty NavigationStateProperty =
        BindingHelper.CreateProperty<ButtonPanel, NavigationState>("NavigationState", null);

    public NavigationState NavigationState
    {
        get { return (NavigationState)GetValue(NavigationStateProperty); }
        set { SetValue(NavigationStateProperty, value); }
    }

    public ButtonPanel()
    {
        InitializeComponent();
    }
}
