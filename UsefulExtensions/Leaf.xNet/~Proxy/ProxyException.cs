using System;
using System.Runtime.Serialization;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Исключение, которое выбрасывается, в случае возникновения ошибки при работе с прокси.
    /// </summary>
    [Serializable]
    public sealed class ProxyException : NetException
    {
        /// <summary>
        /// Возвращает прокси-клиент, в котором произошла ошибка.
        /// </summary>
        public ProxyClient ProxyClient { get; }

        
        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.ProxyException" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public ProxyException() : this(Resources.ProxyException_Default) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.ProxyException" /> заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
        /// <param name="innerException">Исключение, вызвавшее текущие исключение, или значение <see langword="null" />.</param>
        public ProxyException(string message, Exception innerException = null)
            : base(message, innerException) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="!:Leaf.xNet.Net.ProxyException" /> заданным сообщением об ошибке и прокси-клиентом.
        /// </summary>
        /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
        /// <param name="proxyClient">Прокси-клиент, в котором произошла ошибка.</param>
        /// <param name="innerException">Исключение, вызвавшее текущие исключение, или значение <see langword="null" />.</param>
        public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
            : base(message, innerException)
        {
            ProxyClient = proxyClient;
        }

        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.ProxyException" /> заданными экземплярами <see cref="T:System.Runtime.Serialization.SerializationInfo" /> и <see cref="T:System.Runtime.Serialization.StreamingContext" />.
        /// </summary>
        /// <param name="serializationInfo">Экземпляр класса <see cref="T:System.Runtime.Serialization.SerializationInfo" />, который содержит сведения, требуемые для сериализации нового экземпляра класса <see cref="T:Leaf.xNet.ProxyException" />.</param>
        /// <param name="streamingContext">Экземпляр класса <see cref="T:System.Runtime.Serialization.StreamingContext" />, содержащий источник сериализованного потока, связанного с новым экземпляром класса <see cref="T:Leaf.xNet.ProxyException" />.</param>
        public ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
    }
}
