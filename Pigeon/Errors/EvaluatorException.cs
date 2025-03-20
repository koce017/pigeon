using System;

namespace Kostic017.Pigeon.Errors
{
    class EvaluatorException : Exception
    {
        internal EvaluatorException(string message) : base(message)
        {
        }
    }
}
