namespace Hud1.Models
{
    public class NavigationStates
    {
        // MENU
        public static readonly NavigationState MENU_GAMMA = new();
        public static readonly NavigationState GAMMA_VISIBLE = new();

        public static readonly NavigationState MENU_AUDIO = new();
        public static readonly NavigationState AUDIO_VISIBLE = new();

        public static readonly NavigationState MENU_MACRO = new();
        public static readonly NavigationState MACRO_VISIBLE = new();

        public static readonly NavigationState MENU_CROSSHAIR = new();
        public static readonly NavigationState CROSSHAIR_VISIBLE = new();

        public static readonly NavigationState MENU_MORE = new();
        public static readonly NavigationState MORE_VISIBLE = new();

        // SOUND
        public static readonly NavigationState PLAYBACK_DEVICE = new() { Label = "Playback Device" };
        public static readonly NavigationState PLAYBACK_VOLUME = new() { Label = "Volume" };
        public static readonly NavigationState PLAYBACK_MUTE = new() { Label = "Mute" };

        // MORE
        public static readonly NavigationState EXIT = new();
        public static readonly NavigationState STYLE = new() { SelectionLabel = "Green" };
    }
}
