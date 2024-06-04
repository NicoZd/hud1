using System.Text;
using Xunit.Abstractions;

namespace Tests.Helpers
{
    internal class RedirectOutput : TextWriter
    {
        private readonly ITestOutputHelper _output;

        internal RedirectOutput(ITestOutputHelper output)
        {
            _output = output;

        }

        public override Encoding Encoding => Encoding.Unicode;

        public override void WriteLine(string? value)
        {
            _output.WriteLine(value);
        }

        public override void Write(string? value)
        {
            _output.WriteLine(value);
        }
    }
}
