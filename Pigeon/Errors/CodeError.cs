namespace Kostic017.Pigeon.Errors
{
    class CodeError
    {
        internal string Message { get; }
        internal TextSpan TextSpan { get; }

        internal CodeError(string message, TextSpan textSpan)
        {
            Message = message;
            TextSpan = textSpan;
        }

        public override string ToString() => $"{TextSpan.Line}:{TextSpan.Column} {Message}";
    }
}
