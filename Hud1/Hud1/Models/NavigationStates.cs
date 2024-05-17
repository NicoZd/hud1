﻿namespace Hud1.Models
{
    public class NavigationStates
    {
        public static readonly NavigationState ALL = new();

        // MENU
        public static readonly NavigationState MENU_DISPLAY = new() { Hint = "Adjust Display Settings." };
        public static readonly NavigationState DISPLAY_VISIBLE = new();

        public static readonly NavigationState MENU_AUDIO = new() { Hint = "Adjust Input/Output Audio Devices." };
        public static readonly NavigationState AUDIO_VISIBLE = new();

        public static readonly NavigationState MENU_MACRO = new() { Hint = "Start Mouse & Keyboard Macros." };
        public static readonly NavigationState MACRO_VISIBLE = new();

        public static readonly NavigationState MENU_CROSSHAIR = new() { Hint = "Enable and configure Crosshair." };
        public static readonly NavigationState CROSSHAIR_VISIBLE = new();

        public static readonly NavigationState MENU_MORE = new() { Hint = "Quit, Skin, Adjust, Info" };
        public static readonly NavigationState MORE_VISIBLE = new();

        // DISPLAY
        public static readonly NavigationState GAMMA = new() { Label = "Gamma", Hint = "Adjust Windows Desktop Gamma Settings.", AllowRepeat = true };

        // SOUND
        public static readonly NavigationState PLAYBACK_DEVICE = new() { Label = "Device", Hint = "Choose Playback Audio Device." };
        public static readonly NavigationState PLAYBACK_VOLUME = new() { Label = "Volume", AllowRepeat = true };
        public static readonly NavigationState PLAYBACK_MUTE = new() { Label = "Mute", SelectionLeftLabel = " < Mute ", SelectionRightLabel = "Unmute >" };

        public static readonly NavigationState CAPTURE_DEVICE = new() { Label = "Device" };
        public static readonly NavigationState CAPTURE_VOLUME = new() { Label = "Volume", AllowRepeat = true };
        public static readonly NavigationState CAPTURE_MUTE = new() { Label = "Mute", SelectionLeftLabel = " < Mute ", SelectionRightLabel = "Unmute >" };

        // MACRO
        public static readonly NavigationState MACROS = new() { };
        public static readonly NavigationState MACROS_FOLDER = new() { Label = "Open Folder", Hint = "Open explorer.exe in Macros Folder." };

        // MORE
        public static readonly NavigationState ACTIVATE = new() { Label = "Activate", Hint = "Activate this Window to capture mouse once." };
        public static readonly NavigationState EXIT = new() { Label = "Exit", Hint = "Exit HUD and terminate process." };
        public static readonly NavigationState HUD_POSITION = new() { Label = "HUD Position", Hint = "Select Position of the HUD" };
        public static readonly NavigationState KEYBOARD_CONTROL = new() { Label = "Keyboard Navigation", Hint = "Capture Cursor Keys to navigate this window.", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };
        public static readonly NavigationState SHOW_HELP = new() { Label = "Help Contents", Hint = "Show hints and top help content.", SelectionLeftLabel = "< Off", SelectionRightLabel = "On >" };
        public static readonly NavigationState STYLE = new() { SelectionLabel = "Green", Label = "Colors", Hint = "Adjust colors of this window." };
        public static readonly NavigationState FONT = new() { SelectionLabel = "Source Code Pro", Label = "Font", Hint = "Select Font" };
    }
}
