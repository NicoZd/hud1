﻿namespace Hud1.Models;

internal class NavigationStates
{
    internal static readonly NavigationState ALL = new();

    // MENU
    internal static readonly NavigationState MENU_NIGHTVISION = new() { Hint = "Press Alt+Shift+N at any time to toggle Nightvision. Adjust the Gamma settings to configure the light amplification." };
    internal static readonly NavigationState NIGHTVISION_VISIBLE = new();

    internal static readonly NavigationState MENU_CROSSHAIR = new() { Hint = "Press Alt+Shift+C at any time to toggle the crosshair. Adjust its form, color, and size here." };
    internal static readonly NavigationState CROSSHAIR_VISIBLE = new();

    internal static readonly NavigationState MENU_MACRO = new() { Hint = "Here you can start and stop your macros. Ensure the correct window is selected for macro execution. See below for further tips on macro development." };
    internal static readonly NavigationState MACRO_VISIBLE = new();

    internal static readonly NavigationState MENU_MORE = new() { Hint = "Quit the application, configure the HUD, and adjust the style" };
    internal static readonly NavigationState MORE_VISIBLE = new();

    // NIGHTVISION
    internal static readonly NavigationState NIGHTVISION_ENABLED = new() { Label = "Toggle", Hint = "Use Alt+Shift+N at any time to toggle Nightvision.", SelectionLeftLabel = "⏴Off", SelectionRightLabel = "On⏵" };
    internal static readonly NavigationState GAMMA = new() { Label = "Gamma", Hint = "Adjust Windows Desktop Gamma while Nightvision is toggled on.", AllowRepeat = true, ValueConverter = new GammaConverter() };

    // CROSSHAIR
    internal static readonly NavigationState CROSSHAIR_ENABLED = new() { Label = "Toggle", SelectionLeftLabel = "⏴Off", SelectionRightLabel = "On⏵", Hint = "Enable or disable the crosshair with Alt+Shift+C." };
    internal static readonly NavigationState CROSSHAIR_FORM = new() { Label = "Form", Hint = "Select Crosshair Form", AllowRepeat = true };
    internal static readonly NavigationState CROSSHAIR_COLOR = new() { Label = "Color", Hint = "Select Crosshair Color", AllowRepeat = true };
    internal static readonly NavigationState CROSSHAIR_OPACITY = new() { Label = "Opacity", Hint = "Select Crosshair Opacity", ValueConverter = new DoubleConverter(), AllowRepeat = true };
    internal static readonly NavigationState CROSSHAIR_SIZE = new() { Label = "Size", Hint = "Select Crosshair Size", AllowRepeat = true };
    internal static readonly NavigationState CROSSHAIR_OUTLINE = new() { Label = "Outline", SelectionLeftLabel = "⏴Off", SelectionRightLabel = "On⏵", Hint = "Enable or disable Crosshair black outline." };

    // MACRO
    internal static readonly NavigationState MACROS = new() { };
    internal static readonly NavigationState MACROS_DEV = new() { };
    internal static readonly NavigationState DEVELOPER_MODE = new() { Label = "Macro Developer Mode", Hint = "In Dev Mode: Arrow Keys are only available if this window is active. The window is visible in Alt-Tab and automatically activated when the mouse is hidden.", SelectionLeftLabel = "⏴Off ", SelectionRightLabel = "On⏵" };
    internal static readonly NavigationState MACROS_FOLDER = new() { Label = "Open Macros folder", Hint = "Start creating and editing Macros by adding new .lua files in the Macros folder. This view will update automatically as you make changes." };

    // MORE
    internal static readonly NavigationState EXIT = new() { Label = "Exit", Hint = "Exit the HUD and terminate the process." };

    internal static readonly NavigationState CROSSHAIR_MONITOR = new() { Label = "Crosshair Display", Hint = "Select the display where the crosshair is shown.", ValueConverter = new DisplayConverter() };
    internal static readonly NavigationState HUD_POSITION = new() { Label = "Window Display and Position", Hint = "Select display and the side of this window.", ValueConverter = new DisplayConverter() };

    internal static readonly NavigationState STYLE = new() { Value = "Green", Label = "Colors", Hint = "Adjust colors of this window." };
    internal static readonly NavigationState FONT = new() { Value = "Fira Code", Label = "Font", Hint = "Select Font. Open the UserData folder (see macros) to add or remove fonts." };
}
