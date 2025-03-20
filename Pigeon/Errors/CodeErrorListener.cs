using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Kostic017.Pigeon.Errors;
using System;

namespace Kostic017.Pigeon
{
    class CodeErrorListener : BaseErrorListener
    {
        private readonly CodeErrorBag errorBag;

        public CodeErrorListener(CodeErrorBag errorBag)
        {
            this.errorBag = errorBag;
        }

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            errorBag.Report(msg, new TextSpan(offendingSymbol.Line, offendingSymbol.Column, offendingSymbol.StartIndex, offendingSymbol.StopIndex));
        }
    }
}
