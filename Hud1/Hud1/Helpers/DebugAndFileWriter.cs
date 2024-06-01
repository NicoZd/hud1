using System.Diagnostics;
using System.IO;
using System.Text;

namespace Hud1.Helpers;

internal class DebugAndFileWriter : TextWriter
{
    private readonly string Path;

    internal DebugAndFileWriter(string path)
    {
        Path = path;
        if (File.Exists(Path))
            File.Delete(Path);
    }

    public override void WriteLine(string? value)
    {
        Debug.WriteLine(value);
        using var fileStream = new FileStream(Path, FileMode.Append);
        using var streamWriter = new StreamWriter(fileStream);
        streamWriter.WriteLine(value);
    }

    public override void Write(string? value)
    {
        Debug.Write(value);
        using var fileStream = new FileStream(Path, FileMode.Append);
        using var streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(value);
    }

    public override Encoding Encoding => Encoding.Unicode;
}
