using Hud1;
using Hud1.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hud1.Tests
{
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
}