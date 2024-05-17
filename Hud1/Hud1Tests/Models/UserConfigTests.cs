﻿using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Hud1.Models.Tests
{
    [TestClass()]
    public class UserConfigTests
    {
        [TestMethod()]
        public void LogTest()
        {
            var config = new UserConfig();
            Debug.WriteLine("hello6");
        }

        [TestMethod()]
        public void CreateDefault()
        {
            var file = new { someInt = 2, someNone = 2 };
            string jsonString = JsonSerializer.Serialize(file);
            Debug.Print("File {0}", jsonString);

            var config = new UserConfig();

            config.someString = "fromSystem";


            var loaded = JsonSerializer.Deserialize<UserConfig>(jsonString);

            foreach (PropertyInfo prop in config.GetType().GetProperties())
            {
                var value = prop.GetValue(loaded, null);
                if (value != null)
                {
                    Debug.Print("Set {0} to {1}", prop.Name, value);
                    prop.SetValue(config, value);
                }
            }

            Assert.AreEqual(config.someInt, 2);
            Assert.AreEqual(config.someString, "two");


            Debug.Print("hello6");
        }

    }
}