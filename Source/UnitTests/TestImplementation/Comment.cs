using Devsense.PHP.Text;

namespace UnitTests.TestImplementation
{
    internal struct Comment
    {
        public readonly Span Position;
        public readonly string Text;
        public Comment(Span pos, string txt) { Position = pos; Text = txt; }
    }
}
