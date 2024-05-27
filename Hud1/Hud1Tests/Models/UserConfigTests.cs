using Hud1.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Hud1Tests.Models;

[TestClass()]
public class UserConfigTests
{
    [TestMethod()]
    public void LogTest()
    {
        _ = new UserConfig();
        Debug.WriteLine("hello6");
    }

    [TestMethod()]
    public void CreateDefault()
    {
        var file = new { GammaIndex = 2, someNone = 2 };
        string jsonString = JsonSerializer.Serialize(file);
        Debug.Print("File {0}", jsonString);

        UserConfig config = new();

        UserConfig? loaded = JsonSerializer.Deserialize<UserConfig>(jsonString);

        foreach (PropertyInfo prop in config.GetType().GetProperties())
        {
            object? value = prop.GetValue(loaded, null);
            if (value != null)
            {
                Debug.Print("Set {0} to {1}", prop.Name, value);
                prop.SetValue(config, value);
            }
        }

        Assert.AreEqual(config.GammaIndex, 2);
        Assert.AreEqual(config.Style, "Green");


        Debug.Print("hello6");
    }

}