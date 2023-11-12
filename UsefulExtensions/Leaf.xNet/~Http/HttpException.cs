using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Исключение, которое выбрасывается, в случае возникновения ошибки при работе с HTTP-протоколом.
    /// </summary>
    [Serializable]
    public sealed class HttpException : NetException
    {
        #region Свойства (открытые)

        /// <summary>
        /// Возвращает состояние исключения.
        /// </summary>
        public HttpExceptionStatus Status { get; internal set; }

        /// <summary>
        /// Возвращает код состояния ответа от HTTP-сервера.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        #endregion


        internal bool EmptyMessageBody { get; set; }


        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpException" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public HttpException() : this(Resources.HttpException_Default) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpException" /> заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
        /// <param name="innerException">Исключение, вызвавшее текущие исключение, или значение <see langword="null" />.</param>
        public HttpException(string message, Exception innerException = null)
            : base(message, innerException) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpException" /> заданным сообщением об ошибке и кодом состояния ответа.
        /// </summary>
        /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
        /// <param name="status">Статус HTTP вызванного исключения</param>
        /// <param name="httpStatusCode">Код состояния ответа от HTTP-сервера.</param>
        /// <param name="innerException">Исключение, вызвавшее текущие исключение, или значение <see langword="null" />.</param>
        public HttpException(string message, HttpExceptionStatus status,
            HttpStatusCode httpStatusCode = HttpStatusCode.None, Exception innerException = null)
            : base(message, innerException)
        {
            Status = status;
            HttpStatusCode = httpStatusCode;
        }

        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpException" /> заданными экземплярами <see cref="T:System.Runtime.Serialization.SerializationInfo" /> и <see cref="T:System.Runtime.Serialization.StreamingContext" />.
        /// </summary>
        /// <param name="serializationInfo">Экземпляр класса <see cref="T:System.Runtime.Serialization.SerializationInfo" />, который содержит сведения, требуемые для сериализации нового экземпляра класса <see cref="T:Leaf.xNet.HttpException" />.</param>
        /// <param name="streamingContext">Экземпляр класса <see cref="T:System.Runtime.Serialization.StreamingContext" />, содержащий источник сериализованного потока, связанного с новым экземпляром класса <see cref="T:Leaf.xNet.HttpException" />.</param>
        public HttpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            if (serializationInfo == null)
                return;

            Status = (HttpExceptionStatus)serializationInfo.GetInt32("Status");
            HttpStatusCode = (HttpStatusCode)serializationInfo.GetInt32("HttpStatusCode");
        }


        /// <inheritdoc />
        /// <summary>
        /// Заполняет экземпляр <see cref="T:System.Runtime.Serialization.SerializationInfo" /> данными, необходимыми для сериализации исключения <see cref="T:Leaf.xNet.HttpException" />.
        /// </summary>
        /// <param name="serializationInfo">Данные о сериализации, <see cref="T:System.Runtime.Serialization.SerializationInfo" />, которые должны использоваться.</param>
        /// <param name="streamingContext">Данные о сериализации, <see cref="T:System.Runtime.Serialization.StreamingContext" />, которые должны использоваться.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);

            serializationInfo.AddValue("Status", (int)Status);
            serializationInfo.AddValue("HttpStatusCode", (int)HttpStatusCode);
            serializationInfo.AddValue("EmptyMessageBody", EmptyMessageBody);
        }
    }
}
