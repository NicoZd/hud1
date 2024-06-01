using System.Windows.Media.Animation;

namespace Hud1.Helpers;

internal static class StoryboardExtensions
{
    internal static Task BeginAsync(this Storyboard storyboard)
    {
        var tcs = new TaskCompletionSource<bool>();

        async void OnCompleted(object? sender, EventArgs e)
        {
            storyboard.Completed -= OnCompleted;
            await Task.Delay(50);
            tcs.SetResult(true);
        }

        storyboard.Completed += OnCompleted;
        storyboard.Begin();
        return tcs.Task;
    }
}

