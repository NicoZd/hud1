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
    private bool _rightEnabled = false;

    [ObservableProperty]
    private bool _leftEnabled = false;

    [ObservableProperty]
    private string _path = "";

    [ObservableProperty]
    private string _fileName = "";

    public int ThreadsRunning => ThreadPool.ThreadsRunning;

    private MacroScript macroScript;
    private readonly MacrosViewModel macros;
    private readonly MacroThreadPool ThreadPool = new();

    public Macro(string path, MacrosViewModel macros)
    {
        Path = path;
        this.macros = macros;

        FileName = System.IO.Path.GetFileName(Path);
        Label = FileName;
        Description = "";
        RightEnabled = true;
        LeftEnabled = false;

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
    private void ToggleClick()
    {
        Console.WriteLine("StartClick");
        macros.SelectMacro(this);
        Toggle();
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
        Toggle();
    }

    public void OnRight()
    {
        Console.WriteLine($"OnRight Running: {Running}");
        Toggle();
    }

    private void Toggle()
    {
        if (Running)
        {
            Stop();
        }
        else
        {

            Error = "";
            Running = true;
            RightEnabled = false;
            LeftEnabled = true;

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
                RightEnabled = true;
                LeftEnabled = false;
            });
        }
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
        RightEnabled = false;
        LeftEnabled = false;

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
            GlobalKeyboardHook.KeyDown += OnKeyDown;
            GlobalKeyboardHook.KeyUp += OnKeyUp;
        }

        public void Dispose()
        {
            Console.WriteLine("MACROHOOK Dispose");
            GlobalMouseHook.MouseDown -= OnMouseDown;
            GlobalMouseHook.MouseUp -= OnMouseUp;
            GlobalKeyboardHook.KeyDown -= OnKeyDown;
            GlobalKeyboardHook.KeyUp -= OnKeyUp;
        }

        private void OnMouseDown(int button, POINT point)
        {
            Console.WriteLine($"OnMouseDown button: {button}");
            _macroScript.OnMouseDown(button, point);
        }

        private void OnMouseUp(int button, POINT point)
        {
            Console.WriteLine($"OnMouseUp button: {button}");
            _macroScript.OnMouseUp(button, point);
        }

        private void OnKeyDown(KeyEvent keyEvent)
        {
            Console.WriteLine($"OnKeyDown keyEvent: {keyEvent.key} {keyEvent.shift} {keyEvent.alt} {keyEvent.repeated}");
            _macroScript.OnKeyDown(keyEvent);
        }

        private void OnKeyUp(KeyEvent keyEvent)
        {
            Console.WriteLine($"OnKeyUp keyEvent: {keyEvent.key} {keyEvent.shift} {keyEvent.alt} {keyEvent.repeated}");
            _macroScript.OnKeyUp(keyEvent);
        }
    }
}
