using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    internal class TempFile
    {
        internal static string CreateTempFile(string content)
        {
            var file = Path.GetTempFileName();
            File.WriteAllText(file, content);
            return file;
        }
    }
}
