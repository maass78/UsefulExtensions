using System;

namespace UsefulExtensions.CaptchaSolvers.Exceptions
{
    /// <summary>
    /// Ошибка при использовании сервиса решении капчи
    /// </summary>
    public class InvalidRequestException : Exception
    {
        /// <summary>
        /// Конструктор <see cref="InvalidRequestException"/>
        /// </summary>
        public InvalidRequestException() { }
        /// <summary>
        /// Конструктор <see cref="InvalidRequestException"/> с сообщением
        /// </summary>
        public InvalidRequestException(string message) : base(message) { }
    }
}
