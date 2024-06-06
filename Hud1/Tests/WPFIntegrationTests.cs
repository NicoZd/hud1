using Hud1;
using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests;

[Collection("App")]
public class WPFIntegrationTests
{
    public WPFIntegrationTests(ITestOutputHelper output, AppFixture f)
    {
        Console.SetOut(new RedirectOutput(output));
    }

    [WpfFact]
    public async Task Setup_FirstRun()
    {
        await Setup.Run();
        Assert.True(Directory.Exists(Setup.RootPath));
        Assert.True(Directory.Exists(Setup.UserDataPath));
    }

    [WpfFact]
    public async Task Setup_RepeatedRun()
    {
        await Setup.Run();
        Assert.True(Directory.Exists(Setup.RootPath));
        Assert.True(Directory.Exists(Setup.UserDataPath));
    }
}
