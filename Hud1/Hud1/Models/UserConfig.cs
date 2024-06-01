namespace Hud1.Models;

public class UserConfig
{
    internal static readonly UserConfig Current = new();

    // Nightvision
    public int GammaIndex { get; set; }

    // Crosshair
    public string CrosshairDisplay { get; set; }

    // More
    public string HudPosition { get; set; }

    public bool TouchModeEnabled { get; set; }

    public string Style { get; set; }

    public string Font { get; set; }

    public UserConfig()
    {
        // Nightvision
        GammaIndex = 4;

        // Crosshair
        CrosshairDisplay = "1";

        // More
        Style = "Green";
        Font = "Fira Code";
        HudPosition = "1:Left";
        TouchModeEnabled = false;
    }

}
