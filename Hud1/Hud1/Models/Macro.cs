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
        try
        {
            var script = new MacroScript(this);
            Label = (string)script.GetGlobal("Label");
            Description = (string)script.GetGlobal("Description");
        }
        catch (InterpreterException ex)
        {
            Error = ex.DecoratedMessage;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    internal void OnLeft()
    {
    }

    internal void OnRight()
    {
        Console.WriteLine("OnRight");
        if (Running && macroScript != null)
        {
            RightLabel = "Stopping";
            macroScript.SetGlobal("Running", false);
            return;
        }

        Error = "";
        Running = true;
        RightLabel = "Stop >";

        ThreadPool.QueueUserWorkItem((_) =>
        {
            try
            {
                macroScript = new MacroScript(this);
                using var hooks = new ScriptHooks(macroScript);
                macroScript.Run();
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
            Running = false;
            RightLabel = "Start >";
        });
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
