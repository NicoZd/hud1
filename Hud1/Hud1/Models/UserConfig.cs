namespace Hud1.Models;

public class UserConfig
{
    internal static readonly UserConfig Current = new();

    // Nightvision
    public int GammaIndex { get; set; } = 4;

    // Crosshair
    public string CrosshairDisplay { get; set; } = "0";

    public bool CrosshairEnabled { get; set; } = true;

    public string CrosshairForm { get; set; } = "Cross";

    public string CrosshairColor { get; set; } = "#FFFFFF";

    public string CrosshairOpacity { get; set; } = "1";

    public string CrosshairSize { get; set; } = "3";

    public bool CrosshairOutline { get; set; } = true;

    // More
    public string HudPosition { get; set; } = "0:Right";

    public bool TouchModeEnabled { get; set; } = false;

    public string Style { get; set; } = "Green";

    public string Font { get; set; } = "Fira Code";

    public UserConfig()
    {
    }

}
