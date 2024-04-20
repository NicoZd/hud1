namespace Hud1.Model
{
    class WindowModel : ObservableObject
    {
        public Boolean _active = false;
        public Boolean Active
        {
            get { return _active; }
            set { _active = value; OnPropertyChanged(); }
        }
    }
}
