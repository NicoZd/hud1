namespace Tests.Helpers
{
    internal class TempFile
    {
        internal static string CreateTempFile(string content)
        {
            string file = Path.GetTempFileName();
            File.WriteAllText(file, content);
            return file;
        }
    }
}
