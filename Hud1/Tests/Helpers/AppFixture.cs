using Hud1;

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