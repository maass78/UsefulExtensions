using System;
using System.Collections.Generic;
using System.Text;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет тело запроса в виде параметров запроса.
    /// </summary>
    public class FormUrlEncodedContent : BytesContent
    {
        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.FormUrlEncodedContent" />.
        /// </summary>
        /// <param name="content">Содержимое тела запроса в виде параметров запроса.</param>
        /// <param name="valuesUnescaped">Указывает, нужно ли пропустить кодирование значений параметров запроса.</param>
        /// <param name="keysUnescaped">Указывает, нужно ли пропустить кодирование имен параметров запроса.</param>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="content" /> равно <see langword="null" />.</exception>
        /// <remarks>По умолчанию используется тип контента - 'application/x-www-form-urlencoded'.</remarks>
        // ReSharper disable once UnusedMember.Global
        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content, bool valuesUnescaped = false, bool keysUnescaped = false)
        {
            #region Проверка параметров

            if (content == null)
                throw new ArgumentNullException(nameof(content));

            #endregion

            Init(Http.ToQueryString(content, valuesUnescaped, keysUnescaped));
        }

        public FormUrlEncodedContent(RequestParams rp)
        {
            #region Проверка параметров

            if (rp == null)
                throw new ArgumentNullException(nameof(rp));

            #endregion

            Init(rp.Query);
        }

        private void Init(string content)
        {
            Content = Encoding.ASCII.GetBytes(content);
            Offset = 0;
            Count = Content.Length;

            MimeContentType = "application/x-www-form-urlencoded";
        }
    }
}
