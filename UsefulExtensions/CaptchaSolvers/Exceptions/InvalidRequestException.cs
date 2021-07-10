using System;

namespace UsefulExtensions.CaptchaSolvers.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException() { }
        public InvalidRequestException(string message) : base(message) { }
    }
}
