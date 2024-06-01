using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hud1.Helpers;
internal class FunctionQueue
{
    private readonly Func<string, Task> function;
    private readonly Queue<string> _runningUpdates = [];
    private bool _running = false;

    public FunctionQueue(Func<string, Task> function)
    {
        this.function = function;
    }

    public async Task Run(string value)
    {
        Debug.Print($"FunctionQueue Run {value}");
        if (_running)
        {
            Debug.Print($"FunctionQueue IsRunning... enqueue {value}");
            _runningUpdates.Enqueue(value);
            return;
        }

        _running = true;

        try
        {
            await function(value);
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }

        _running = false;

        if (_runningUpdates.Count > 0)
        {
            var next = _runningUpdates.Dequeue();
            Debug.Print($"FunctionQueue Run from queue... next: {next}");
            await Run(next);
        }
    }
}
