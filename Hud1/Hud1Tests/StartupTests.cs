﻿using System.Diagnostics;

namespace Hud1Tests;

[TestClass()]
public class StartupTests
{
    [TestMethod()]
    public async Task RunTest_ShouldCompleteSuccessfully()
    {
        Debug.WriteLine("Source:  StartupTests.cs  line 5");

        await Startup.Run();

        Assert.IsTrue(true);
    }
}