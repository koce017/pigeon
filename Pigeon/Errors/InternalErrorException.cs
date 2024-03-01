using System;

namespace Kostic017.Pigeon
{
    class InternalErrorException : Exception
    {
        internal InternalErrorException(string message) : base(message)
        {
        }
    }
}