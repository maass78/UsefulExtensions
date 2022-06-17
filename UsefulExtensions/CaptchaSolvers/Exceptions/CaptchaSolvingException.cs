using System;
using System.Runtime.Serialization;

namespace UsefulExtensions.CaptchaSolvers.Exceptions
{
    /// <summary>
    /// Исключение, которое возникает при ошибке решения капчи сервисом
    /// </summary>
    public class CaptchaSolvingException : Exception
    {
        /// <summary>
        /// ID ошибки на сервисе решения капчи
        /// </summary>
        /// <remarks>
        /// ID могут отличаться в зависимости от сервиса, который вы используете
        /// </remarks>
        public int ErrorId { get; }

        /// <summary>
        /// Сервис решения капчи, который вызвал это исключение
        /// </summary>
        public ICaptchaSolver CaptchaSolver { get; }

        public CaptchaSolvingException(int errorId, ICaptchaSolver solver)
        {
            ErrorId = errorId;
            CaptchaSolver = solver;
        }

        public CaptchaSolvingException(int errorId, ICaptchaSolver solver, string message) : base(message)
        {
            ErrorId = errorId;
            CaptchaSolver = solver;
        }

        public CaptchaSolvingException(int errorId, ICaptchaSolver solver, string message, Exception innerException) : base(message, innerException)
        {
            ErrorId = errorId;
            CaptchaSolver = solver;
        }

        protected CaptchaSolvingException(int errorId, ICaptchaSolver solver, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ErrorId = errorId;
            CaptchaSolver = solver;
        }
    }
}
