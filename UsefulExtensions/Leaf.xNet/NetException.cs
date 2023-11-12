using System;
using System.Runtime.Serialization;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Исключение, которое выбрасывается, в случае возникновения ошибки при работе с сетью.
    /// </summary>
    [Serializable]
    public class NetException : Exception
    {
        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.NetException" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public NetException() : this(Resources.NetException_Default) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.NetException" /> заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
        /// <param name="innerException">Исключение, вызвавшее текущие исключение, или значение <see langword="null" />.</param>
        public NetException(string message, Exception innerException = null)
            : base(message, innerException) { }

        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.NetException" /> заданными экземплярами <see cref="T:System.Runtime.Serialization.SerializationInfo" /> и <see cref="T:System.Runtime.Serialization.StreamingContext" />.
        /// </summary>
        /// <param name="serializationInfo">Экземпляр класса <see cref="T:System.Runtime.Serialization.SerializationInfo" />, который содержит сведения, требуемые для сериализации нового экземпляра класса <see cref="T:Leaf.xNet.NetException" />.</param>
        /// <param name="streamingContext">Экземпляр класса <see cref="T:System.Runtime.Serialization.StreamingContext" />, содержащий источник сериализованного потока, связанного с новым экземпляром класса <see cref="T:Leaf.xNet.NetException" />.</param>
        protected NetException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
    }
}
