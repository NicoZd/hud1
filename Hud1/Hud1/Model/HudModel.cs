namespace Hud1.Model
{
    public class HudModel : ObservableObject
    {
        public String _state = "State";
        public String State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }

        //panel-left, panel-right, panel-top, panel-bottom
        public String _panel = "Panel";
        public String Panel
        {
            get { return _panel; }
            set { _panel = value; OnPropertyChanged(); }
        }

        public Dictionary<string, object> _states = new Dictionary<string, object> { };
        public Dictionary<string, object> States
        {
            get { return _states; }
            set { _states = value; OnPropertyChanged(); }
        }
    }
}
