using System.Diagnostics;

namespace Hud1.Helpers;
internal class FunctionDebounce
{
    private readonly Func<string, Task> function;
    private string next = string.Empty;
    private bool running = false;

    public FunctionDebounce(Func<string, Task> function)
    {
        this.function = function;
    }

    public async Task Run(string value)
    {
        Debug.Print($"FunctionDebounce Run {value}");
        if (running)
        {
            Debug.Print($"FunctionDebounce set next: {value}");
            next = value;
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

        if (!string.Empty.Equals(next))
        {
            var next = this.next;
            this.next = string.Empty;
            Debug.Print($"FunctionDebounce Run next: {next}");
            await Run(next);
        }
    }
}
