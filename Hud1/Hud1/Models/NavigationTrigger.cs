using System.Runtime.CompilerServices;

namespace Hud1.Models;

public class NavigationTrigger
{
    public string Label { get; set; }

    public NavigationTrigger([CallerMemberName] string label = "")
    {
        Label = label;
    }

    public override string? ToString()
    {
        return Label;
    }
}
