namespace Hud1.Models
{
    public class NavigationStates
    {
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

        // MORE
        public static readonly NavigationState EXIT = new();
        public static readonly NavigationState STYLE = new() { SelectionLabel = "Green", Label = "Colors", Hint = "Adjust colors of this window." };
    }
}
