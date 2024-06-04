using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests;

[Collection("App")]
public class OutputTests
{
    public OutputTests(ITestOutputHelper output)
    {
        Console.SetOut(new RedirectOutput(output));
    }

    [Fact]
    public void Console_WriteLine()
    {
        Console.WriteLine("Some Output");
        Assert.True(true);
    }
}
