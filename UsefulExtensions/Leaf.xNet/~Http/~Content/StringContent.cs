using System;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет тело запроса в виде строки.
    /// </summary>
    public class StringContent : BytesContent
    {
        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.StringContent" />.
        /// </summary>
        /// <param name="content">Содержимое контента.</param>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="content" /> равно <see langword="null" />.</exception>
        /// <remarks>По умолчанию используется тип контента - 'text/plain'.</remarks>
        public StringContent(string content)
            : this(content, Encoding.UTF8) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.StringContent" />.
        /// </summary>
        /// <param name="content">Содержимое контента.</param>
        /// <param name="encoding">Кодировка, применяемая для преобразования данных в последовательность байтов.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Значение параметра <paramref name="content" /> равно <see langword="null" />.
        /// -или-
        /// Значение параметра <paramref name="encoding" /> равно <see langword="null" />.
        /// </exception>
        /// <remarks>По умолчанию используется тип контента - 'text/plain'.</remarks>
        public StringContent(string content, Encoding encoding)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            Content = encoding?.GetBytes(content) ?? throw new ArgumentNullException(nameof(encoding));
            Offset = 0;
            Count = Content.Length;

            MimeContentType = "text/plain";
        }

        #endregion
    }
}
