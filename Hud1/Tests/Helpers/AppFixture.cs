using Hud1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit.Abstractions;

public class AppFixture : IDisposable
{
    public double id;

    public AppFixture()
    {
        if (!App.Testing)
        {
            App.Testing = true;
            App _ = new();
        }
    }

    public void Dispose()
    {
    }
}

[CollectionDefinition("App")]
public class DatabaseCollection : ICollectionFixture<AppFixture>
{
}