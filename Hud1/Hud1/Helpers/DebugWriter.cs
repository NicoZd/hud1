using System.Diagnostics;
using System.IO;
using System.Text;

namespace Hud1.Helpers;

internal class DebugWriter : TextWriter
{
    internal DebugWriter()
    {
    }

    public override void WriteLine(string? value)
    {
        Debug.WriteLine(value);
    }

    public override void Write(string? value)
    {
        Debug.Write(value);
    }

    public override Encoding Encoding => Encoding.Unicode;
}
