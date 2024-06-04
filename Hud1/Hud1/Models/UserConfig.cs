namespace Hud1.Models;

public class UserConfig
{
    internal static readonly UserConfig Current = new();

    // Nightvision
    public int GammaIndex { get; set; } = 4;

    // Crosshair
    public string CrosshairDisplay { get; set; } = "0";

    public bool CrosshairEnabled { get; set; } = true;

    // More
    public string HudPosition { get; set; } = "1:Left";

    public bool TouchModeEnabled { get; set; } = false;

    public string Style { get; set; } = "Green";

    public string Font { get; set; } = "Fira Code";

    public UserConfig()
    {
    }

}
