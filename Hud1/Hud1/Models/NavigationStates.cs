namespace Hud1.Models;

internal class NavigationStates
{
    internal static readonly NavigationState ALL = new();

    // MENU
    internal static readonly NavigationState MENU_NIGHTVISION = new() { Hint = "Use Alt+Shift+N anytime to toogle Nightvision. Adjust Gamma Settings to select intensity." };
    internal static readonly NavigationState NIGHTVISION_VISIBLE = new();

    internal static readonly NavigationState MENU_MACRO = new() { Hint = "Start Mouse & Keyboard Macros." };
    internal static readonly NavigationState MACRO_VISIBLE = new();

    internal static readonly NavigationState MENU_CROSSHAIR = new() { Hint = "Toggle (Alt+Shift+C) and configure Crosshair." };
    internal static readonly NavigationState CROSSHAIR_VISIBLE = new();

    internal static readonly NavigationState MENU_MORE = new() { Hint = "Quit, Skin, Adjust, Info" };
    internal static readonly NavigationState MORE_VISIBLE = new();

    // NIGHTVISION
    internal static readonly NavigationState NIGHTVISION_ENABLED = new() { Label = "Toggle", Hint = "Use Alt+Shift+N anytime to toogle Nightvision.", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };
    internal static readonly NavigationState GAMMA = new() { Label = "Gamma", Hint = "Adjust Windows Desktop Gamma while Nightvision toggled on.", AllowRepeat = true };

    // CROSSHAIR
    internal static readonly NavigationState CROSSHAIR_ENABLED = new() { Label = "Toggle", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >", Hint = "Enable or disable Crosshair (Alt+Shift+C)" };
    internal static readonly NavigationState CROSSHAIR_DISPLAY = new() { Label = "Monitor", Hint = "Select Monitor where Crosshair is shown." };
    internal static readonly NavigationState CROSSHAIR_FORM = new() { Label = "Form", Hint = "Select Crosshair Form" };
    internal static readonly NavigationState CROSSHAIR_COLOR = new() { Label = "Color", Hint = "Select Crosshair Color" };
    internal static readonly NavigationState CROSSHAIR_OPACITY = new() { Label = "Opacity", Hint = "Select Crosshair Opacity" };
    internal static readonly NavigationState CROSSHAIR_SIZE = new() { Label = "Size", Hint = "Select Crosshair Size" };
    internal static readonly NavigationState CROSSHAIR_OUTLINE = new() { Label = "Outline", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >", Hint = "Enable or disable Crosshair black outline." };

    // MACRO
    internal static readonly NavigationState MACROS = new() { };
    internal static readonly NavigationState MACROS_FOLDER = new() { Label = "Open Folder", Hint = "Open explorer.exe in Macros Folder." };

    // MORE
    internal static readonly NavigationState EXIT = new() { Label = "Exit", Hint = "Exit HUD and terminate process." };

    internal static readonly NavigationState HUD_POSITION = new() { Label = "HUD Position", Hint = "Select Position of the HUD" };
    internal static readonly NavigationState TOUCH_MODE = new() { Label = "Touch Mode", Hint = "If enabled this window will not be activated on mouse or touch interaction.", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };

    internal static readonly NavigationState STYLE = new() { SelectionLabel = "Green", Label = "Colors", Hint = "Adjust colors of this window." };
    internal static readonly NavigationState FONT = new() { SelectionLabel = "Fira Code", Label = "Font", Hint = "Select Font" };
}
