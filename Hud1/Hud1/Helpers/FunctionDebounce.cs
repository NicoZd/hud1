using System.Diagnostics;

namespace Hud1.Helpers;
internal class FunctionDebounce<T>
{
    class NextValue
    {
        internal T Value;

        internal NextValue(T value2)
        {
            Value = value2;
        }
    }

    private readonly Func<T, Task> function;

    private NextValue? next = null;

    private bool running = false;

    internal FunctionDebounce(Func<T, Task> function)
    {
        this.function = function;
    }

    internal async Task Run(T value)
    {
        // Debug.Print($"FunctionDebounce Run {value}");
        if (running)
        {
            // Debug.Print($"FunctionDebounce set next: {value}");
            next = new NextValue(value);
            return;
        }

        running = true;

        try
        {
            await function(value);
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }

        running = false;

        if (next != null)
        {
            T nextValue = next.Value;
            next = null;
            // Debug.Print($"FunctionDebounce Run next: {next}");
            await Run(nextValue);
        }
    }
}
