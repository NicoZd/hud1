using Hud1.Models;
using Hud1.ViewModels;
using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests.Models;

[Collection("App")]
public class MacroTests
{
    public MacroTests(ITestOutputHelper output)
    {
        Console.SetOut(new RedirectOutput(output));
    }

    [WpfFact]
    public async Task Macro_Loading()
    {
        string luaFile = TempFile.CreateTempFile(@"
            Label = ""Foo""
            Description = ""Bar""
        ");

        Macro macro = new(luaFile, MacrosViewModel.Instance);
        await Task.Delay(100);

        Assert.Equal("Foo", macro.Label);
        Assert.Equal("Bar", macro.Description);
    }

    [WpfFact]
    public async Task Macro_EmptyLifecycle()
    {
        string luaFile = TempFile.CreateTempFile(@"");

        Macro macro = new(luaFile, MacrosViewModel.Instance);
        Assert.Equal(1, macro.ThreadsRunning);
        Assert.False(macro.Running);

        await Task.Delay(100);
        Assert.Equal(0, macro.ThreadsRunning);
        Assert.False(macro.Running);

        macro.OnRight();
        await Task.Delay(100);

        Assert.Equal(1, macro.ThreadsRunning);
        Assert.True(macro.Running);

        macro.OnRight();
        await Task.Delay(100);

        Assert.Equal(0, macro.ThreadsRunning);
        Assert.False(macro.Running);
    }

    [WpfFact]
    public async Task Macro_EmptyLifecycleShort()
    {
        string luaFile = TempFile.CreateTempFile(@"");

        Macro macro = new(luaFile, MacrosViewModel.Instance);
        Assert.False(macro.Running);

        macro.OnRight();
        Assert.True(macro.Running);

        macro.OnRight();
        await Task.Delay(100);

        Assert.False(macro.Running);
        Assert.Equal(0, macro.ThreadsRunning);
    }

    [WpfFact]
    public async Task Macro_RunAbort()
    {
        string luaFile = TempFile.CreateTempFile(@"
            function Run()	
	            while true do
	            end
            end
        ");

        Macro macro = new(luaFile, MacrosViewModel.Instance);
        Assert.False(macro.Running, "Before Start");

        macro.OnRight();
        await Task.Delay(100);
        Assert.True(macro.Running, "After Start");

        macro.OnRight();
        Assert.True(macro.Running, "After Stop");

        await Task.Delay(2000);
        Assert.Equal("Stopping", macro.RightLabel);

        await Task.Delay(2000);
        Assert.Equal(MacroErrors.ABORT, macro.Error);
    }

    [WpfFact]
    public async Task Macro_InitAbort()
    {
        string luaFile = TempFile.CreateTempFile(@"
	        while true do
	        end
        ");

        Macro macro = new(luaFile, MacrosViewModel.Instance);

        await Task.Delay(100);
        Assert.Equal(1, macro.ThreadsRunning);

        macro.Stop();

        await Task.Delay(2000);
        Assert.Equal(1, macro.ThreadsRunning);

        await Task.Delay(2000);
        Assert.Equal(0, macro.ThreadsRunning);
    }

}
