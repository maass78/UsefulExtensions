using System;
using System.Runtime.Serialization;

namespace UsefulExtensions.CaptchaSolvers.Exceptions
{
    /// <summary>
    /// Ошибка при решении произвольного задания AntiCaptcha в связи с отменой задания работником или по условию.
    /// <br/>
    /// <see cref="Implementations.AnticaptchaSolver.SolveCustomCaptcha{T}(string, string, T, string, int?, string, string, System.Collections.Generic.List{string})"/>
    /// </summary>
    public class CustomCaptchaSolvingException : CaptchaSolvingException
    {
        /// <summary>
        /// Исходное сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Ссылка на скриншот, который приходит при отмене
        /// </summary>
        public string ScreenshotUrl { get; set; }

        public CustomCaptchaSolvingException(int errorId, ICaptchaSolver solver) : base(errorId, solver) { }

        public CustomCaptchaSolvingException(int errorId, ICaptchaSolver solver, string message) : base(errorId, solver, message) { }

        public CustomCaptchaSolvingException(int errorId, ICaptchaSolver solver, string message, Exception innerException) : base(errorId, solver, message, innerException) { }

        protected CustomCaptchaSolvingException(int errorId, ICaptchaSolver solver, SerializationInfo info, StreamingContext context) : base(errorId, solver, info, context) { }
    }
}
