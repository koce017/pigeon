using Antlr4.Runtime;

namespace Kostic017.Pigeon
{
    public class TextSpan
    {
        public int Line { get; }
        public int Column { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }
 
        internal TextSpan(int line, int column, int startIndex, int endIndex)
        {
            Line = line;
            Column = column;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }

    static class TextSpanExt
    {
        internal static TextSpan GetTextSpan(this IToken token)
        {
            return new TextSpan(token.Line, token.Column, token.StartIndex, token.StopIndex);
        }

        internal static TextSpan GetTextSpan(this ParserRuleContext context)
        {
            return new TextSpan(context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Stop.StopIndex);
        }
    }
}
