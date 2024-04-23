using System.Runtime.CompilerServices;
using System.Windows;

namespace Hud1.Models
{
    public partial class NavigationState
    {
        public bool Selected { get; set; }

        public bool SelectRight { get; set; }

        public Visibility Visibility { get; set; }

        public string Label { get; set; }

        public NavigationState([CallerMemberName] string label = "")
        {
            this.Label = label;
            Selected = false;
            SelectRight = false;
            Visibility = Visibility.Collapsed;
        }

        public override string? ToString()
        {
            return Label;
        }
    }
}
