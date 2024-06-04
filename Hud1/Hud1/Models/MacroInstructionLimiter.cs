using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

namespace Hud1.Models;

internal class TooManyInstructions : Exception
{
    internal TooManyInstructions()
    {
    }
}


internal class MacroInstructionLimiter : IDebugger
{
    internal bool Abort = false;

    internal DebuggerAction stepIn = new()
    {
        Action = DebuggerAction.ActionType.StepIn
    };

    public void SetSourceCode(SourceCode sourceCode)
    {
    }

    public void SetByteCode(string[] byteCode)
    {
    }

    public bool IsPauseRequested()
    {
        return true;
    }

    public bool SignalRuntimeException(ScriptRuntimeException ex)
    {
        return false;
    }

    public DebuggerAction GetAction(int ip, SourceRef sourceref)
    {
        return Abort ? throw new TooManyInstructions() : stepIn;
    }

    public void SignalExecutionEnded()
    {
    }

    public void Update(WatchType watchType, IEnumerable<WatchItem> items)
    {
    }

    public List<DynamicExpression> GetWatchItems()
    {
        return [];
    }

    public void RefreshBreakpoints(IEnumerable<SourceRef> refs)
    {
    }

    DebuggerCaps IDebugger.GetDebuggerCaps()
    {
        return DebuggerCaps.CanDebugSourceCode;
    }

    void IDebugger.SetDebugService(DebugService debugService)
    {
    }

}
