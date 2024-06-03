using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.ViewModels;
using MoonSharp.Interpreter;

namespace Hud1.Models;

internal partial class Macro : ObservableObject
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

    private MacroScript? macroScript;
    private readonly MacrosViewModel macros;

    internal Macro(string path, MacrosViewModel macros)
    {
        Path = path;
        Label = System.IO.Path.GetFileName(Path);
        Description = "";
        RightLabel = "Start >";

        this.macros = macros;

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
        ThreadPool.QueueUserWorkItem((_) =>
        {
            RunScript(() =>
            {
                var script = new MacroScript(this);
                Label = (string)script.GetGlobal("Label");
                Description = (string)script.GetGlobal("Description");
            });
        });
    }

    internal void OnLeft()
    {
    }

    internal void OnRight()
    {
        Console.WriteLine("OnRight");
        if (Running)
        {
            Stop();
            return;
        }

        Error = "";
        Running = true;
        RightLabel = "Stop >";

        ThreadPool.QueueUserWorkItem((_) =>
        {
            RunScript(() =>
            {
                macroScript = new MacroScript(this);
                using var hooks = new ScriptHooks(macroScript);
                macroScript.Run();
            });

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
            Error = "Script aborted after waiting 3 seconds to finish by itself.";
        }
        catch (InterpreterException ex)
        {
            Console.WriteLine("ERROR {0}", ex.DecoratedMessage);

            Error = ex.DecoratedMessage ?? "ERROR: " + ex.Message;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    internal void Stop()
    {
        RightLabel = "Stopping";
        if (macroScript != null)
        {
            macroScript.Stop();
        }
    }

    internal class ScriptHooks : IDisposable
    {
        private readonly MacroScript _macroScript;

        internal ScriptHooks(MacroScript macroScript)
        {
            _macroScript = macroScript;
            GlobalMouseHook.MouseDown += OnMouseDown;
        }

        public void Dispose()
        {
            Console.WriteLine("MACROHOOK Dispose");
            GlobalMouseHook.MouseDown -= OnMouseDown;
        }

        private void OnMouseDown()
        {
            Console.WriteLine("OnMouseDown");
            _macroScript.OnMouseDown();
        }
    }
}
