using System.Runtime.CompilerServices;

namespace Hud1.Models;

internal class NavigationTrigger
{
    internal string Label { get; set; }

    internal NavigationTrigger([CallerMemberName] string label = "")
    {
        Label = label;
    }

    public override string? ToString()
    {
        return Label;
    }
}
