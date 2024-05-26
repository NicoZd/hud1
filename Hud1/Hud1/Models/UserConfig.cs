namespace Hud1.Models;

public class UserConfig
{
    public static readonly UserConfig Current = new();

    public UserConfig()
    {
        HudPosition = "0:Right";
        GammaIndex = 4;
        TouchModeEnabled = false;

        Style = "Green";
        Font = "Source Code Pro";
    }

    public int GammaIndex { get; set; }

    public string HudPosition { get; set; }

    public bool TouchModeEnabled { get; set; }

    public string Style { get; set; }

    public string Font { get; set; }
}
