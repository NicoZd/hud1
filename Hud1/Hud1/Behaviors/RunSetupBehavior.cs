using Hud1.Helpers;
using Hud1.ViewModels;
using Hud1.Windows;
using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace Hud1.Behaviors;

internal class RunSetupBehavior : Behavior<SplashWindow>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Debug.Print($"RunSetupBehavior FadeIn {Entry.Millis()}");

        var window = AssociatedObject;

        await ((Storyboard)window.FindResource("FadeIn")).BeginAsync();

        try
        {
            Debug.Print($"RunSetupBehavior Run {Entry.Millis()}");
            await Setup.Run();

            Debug.Print($"RunSetupBehavior Create Windows {Entry.Millis()}");
            MainWindow.Create();
            CrosshairWindow.Create();

            Debug.Print($"RunSetupBehavior Fade Out {Entry.Millis()}");
            await ((Storyboard)window.FindResource("FadeOut")).BeginAsync();
            SplashWindowViewModel.Instance.IsCloseActivated = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            window.Opacity = 0;
            MessageBox.Show("Wooo - there was a fatal startup error:\n\n" + ex.ToString(), "Game Director", MessageBoxButton.OK, MessageBoxImage.Error);
            SplashWindowViewModel.Instance.IsCloseActivated = true;
        }
    }
}
