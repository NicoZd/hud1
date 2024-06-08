using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.ViewModels;
using MoonSharp.Interpreter;

namespace Hud1.Models;

public class MacroErrors
{
    public static readonly string ABORT = "Script aborted after waiting 3 seconds to finish by itself.";
}

public class MacroThreadPool
{
    public int ThreadsRunning { get; set; } = 0;

    public void Run(Action action)
    {
        ThreadsRunning++;
        ThreadPool.QueueUserWorkItem((_) =>
        {
            action();
            ThreadsRunning--;
        });
    }
}

public partial class Macro : ObservableObject
{
    [ObservableProperty]
    private string _label = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _log = "";

    [ObservableProperty]
    private string _error = "";

    [ObservableProperty]
    private bool _selected = false;

    [ObservableProperty]
    private bool _running = false;

    [ObservableProperty]
    private string _rightLabel = "";

    [ObservableProperty]
    private string _path = "";

    public int ThreadsRunning => ThreadPool.ThreadsRunning;

    private MacroScript macroScript;
    private readonly MacrosViewModel macros;
    private readonly MacroThreadPool ThreadPool = new();

    public Macro(string path, MacrosViewModel macros)
    {
        Path = path;
        this.macros = macros;

        Label = System.IO.Path.GetFileName(Path);
        Description = "";
        RightLabel = "Start >";

        macroScript = new MacroScript(this);
        FetchProgramMetaData();
    }

    [RelayCommand]
    private void PanelClick()
    {
        Console.WriteLine("PanelClick");
        macros.SelectMacro(this);
    }

    [RelayCommand]
    private void StartStopClick()
    {
        Console.WriteLine("StartStopClick");
        macros.SelectMacro(this);
        OnRight();
    }

    private void FetchProgramMetaData()
    {
        var local = macroScript;
        ThreadPool.Run(() =>
        {
            RunScript(() =>
            {
                local.ApplyFile();
                Label = (string)macroScript.GetGlobal("Label");
                Description = (string)macroScript.GetGlobal("Description");
            });
        });
    }

    internal void OnLeft()
    {
    }

    public void OnRight()
    {
        Console.WriteLine($"OnRight Running: {Running}");
        if (Running)
        {
            Stop();
            return;
        }

        Error = "";
        Running = true;
        RightLabel = "Stop >";

        macroScript.Stop();
        var local = macroScript = new MacroScript(this);

        ThreadPool.Run(() =>
        {
            RunScript(() =>
            {
                local.ApplyFile();
                using var _ = new ScriptHooks(local);
                local.Run();
            });

            Console.WriteLine("Run Complete...");
            Running = false;
            RightLabel = "Start >";
        });
    }

    private void RunScript(Action scriptAction)
    {
        try
        {
            scriptAction();
        }
        catch (TooManyInstructions ex)
        {
            Console.WriteLine("ERROR {0}", ex.Message);
            Error = MacroErrors.ABORT;
        }
        catch (InterpreterException ex)
        {
            Console.WriteLine("ERROR {0}", ex.DecoratedMessage);
            Error = ex.DecoratedMessage ?? "ERROR: " + ex.Message;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR {0}", ex.Message);
            Error = ex.Message;
        }
    }

    public void Stop()
    {
        RightLabel = "Stopping";
        macroScript?.Stop();
    }

    internal class ScriptHooks : IDisposable
    {
        private readonly MacroScript _macroScript;

        internal ScriptHooks(MacroScript macroScript)
        {
            _macroScript = macroScript;
            GlobalMouseHook.MouseDown += OnMouseDown;
            GlobalMouseHook.MouseUp += OnMouseUp;
        }

        public void Dispose()
        {
            Console.WriteLine("MACROHOOK Dispose");
            GlobalMouseHook.MouseDown -= OnMouseDown;
            GlobalMouseHook.MouseUp -= OnMouseUp;
        }

        private void OnMouseDown(int button)
        {
            Console.WriteLine($"OnMouseDown button: {button}");
            _macroScript.OnMouseDown(button);
        }

        private void OnMouseUp(int button)
        {
            Console.WriteLine($"OnMouseUp button: {button}");
            _macroScript.OnMouseUp(button);
        }
    }
}
