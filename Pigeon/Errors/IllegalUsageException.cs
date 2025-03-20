using System;

namespace Kostic017.Pigeon.Errors
{
    class IllegalUsageException : Exception
    {
        internal IllegalUsageException(string message) : base(message)
        {
        }
    }
}
