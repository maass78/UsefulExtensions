using System;
using System.IO;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет тело запроса. Освобождается сразу после отправки.
    /// </summary>
    public abstract class HttpContent : IDisposable
    {
        /// <summary>MIME-тип контента.</summary>
        protected string MimeContentType = string.Empty;


        /// <summary>
        /// Возвращает или задаёт MIME-тип контента.
        /// </summary>
        public string ContentType
        {
            get => MimeContentType;
            set => MimeContentType = value ?? string.Empty;
        }


        #region Методы (открытые)

        /// <summary>
        /// Подсчитывает и возвращает длину тела запроса в байтах.
        /// </summary>
        /// <returns>Длина тела запроса в байтах.</returns>
        public abstract long CalculateContentLength();

        /// <summary>
        /// Записывает данные тела запроса в поток.
        /// </summary>
        /// <param name="stream">Поток, куда будут записаны данные тела запроса.</param>
        public abstract void WriteTo(Stream stream);

        /// <inheritdoc />
        /// <summary>
        /// Освобождает все ресурсы, используемые текущим экземпляром класса <see cref="T:Leaf.xNet.HttpContent" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion


        /// <summary>
        /// Освобождает неуправляемые (а при необходимости и управляемые) ресурсы, используемые объектом <see cref="HttpContent"/>.
        /// </summary>
        /// <param name="disposing">Значение <see langword="true"/> позволяет освободить управляемые и неуправляемые ресурсы; значение <see langword="false"/> позволяет освободить только неуправляемые ресурсы.</param>
        protected virtual void Dispose(bool disposing) { }
    }
}
