using Hud1;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using Tests.Helpers;
using Xunit;
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
        Assert.True(Directory.Exists(Setup.VersionPath));
    }

    [WpfFact]
    public async Task Setup_RepeatedRun()
    {
        await Setup.Run();
        Assert.True(Directory.Exists(Setup.RootPath));
        Assert.True(Directory.Exists(Setup.VersionPath));
    }
}
