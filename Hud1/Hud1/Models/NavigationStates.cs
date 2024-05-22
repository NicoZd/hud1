namespace Hud1.Models;

public class NavigationStates
{
    public static readonly NavigationState ALL = new();

    // MENU
    public static readonly NavigationState MENU_NIGHTVISION = new() { Hint = "Use F3 anytime to toogle Nightvision. Adjust Gamma Settings to select intensity." };
    public static readonly NavigationState NIGHTVISION_VISIBLE = new();

    public static readonly NavigationState MENU_MACRO = new() { Hint = "Start Mouse & Keyboard Macros." };
    public static readonly NavigationState MACRO_VISIBLE = new();

    public static readonly NavigationState MENU_CROSSHAIR = new() { Hint = "Enable and configure Crosshair." };
    public static readonly NavigationState CROSSHAIR_VISIBLE = new();

    public static readonly NavigationState MENU_MORE = new() { Hint = "Quit, Skin, Adjust, Info" };
    public static readonly NavigationState MORE_VISIBLE = new();

    // NIGHTVISION
    public static readonly NavigationState NIGHTVISION_ENABLED = new() { Label = "Toggle", Hint = "Use F3 anytime to toogle Nightvision.", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };
    public static readonly NavigationState GAMMA = new() { Label = "Gamma", Hint = "Adjust Windows Desktop Gamma while Nightvision toggled on.", AllowRepeat = true };

    // CROSSHAIR
    public static readonly NavigationState CROSSHAIR_ENABLED = new() { Label = "Toggle", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };
    public static readonly NavigationState CROSSHAIR_FORM = new() { Label = "Form" };
    public static readonly NavigationState CROSSHAIR_COLOR = new() { Label = "Color" };
    public static readonly NavigationState CROSSHAIR_SIZE = new() { Label = "Size" };
    public static readonly NavigationState CROSSHAIR_OUTLINE = new() { Label = "Outline", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };

    // MACRO
    public static readonly NavigationState MACROS = new() { };
    public static readonly NavigationState MACROS_FOLDER = new() { Label = "Open Folder", Hint = "Open explorer.exe in Macros Folder." };

    // MORE
    public static readonly NavigationState EXIT = new() { Label = "Exit", Hint = "Exit HUD and terminate process." };
    public static readonly NavigationState ACTIVATE = new() { Label = "Capture Mouse", Hint = "Activate this Window to capture mouse once." };

    public static readonly NavigationState HUD_POSITION = new() { Label = "HUD Position", Hint = "Select Position of the HUD" };
    public static readonly NavigationState KEYBOARD_CONTROL = new() { Label = "Keyboard Navigation", Hint = "Capture Cursor Keys to navigate this window.", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };

    public static readonly NavigationState STYLE = new() { SelectionLabel = "Green", Label = "Colors", Hint = "Adjust colors of this window." };
    public static readonly NavigationState FONT = new() { SelectionLabel = "Source Code Pro", Label = "Font", Hint = "Select Font" };
}
