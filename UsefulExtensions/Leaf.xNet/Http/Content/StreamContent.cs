using System;
using System.IO;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет тело запроса в виде потока.
    /// </summary>
    public class StreamContent : HttpContent
    {
        #region Поля (защищённые электромагнитным излучением)

        /// <summary>Содержимое тела запроса.</summary>
        protected Stream ContentStream;
        /// <summary>Размер буфера в байтах для потока.</summary>
        protected int BufferSize;
        /// <summary>Позиция в байтах, с которой начинается считывание данных из потока.</summary>
        protected long InitialStreamPosition;

        #endregion


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamContent"/>.
        /// </summary>
        /// <param name="contentStream">Содержимое тела запроса.</param>
        /// <param name="bufferSize">Размер буфера в байтах для потока.</param>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="contentStream"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Поток <paramref name="contentStream"/> не поддерживает чтение или перемещение позиции.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> Значение параметра <paramref name="bufferSize"/> меньше 1.</exception>
        /// <remarks>По умолчанию используется тип контента - 'application/octet-stream'.</remarks>
        public StreamContent(Stream contentStream, int bufferSize = 32768)
        {
            #region Проверка параметров

            if (contentStream == null)
                throw new ArgumentNullException(nameof(contentStream));

            if (!contentStream.CanRead || !contentStream.CanSeek)
                throw new ArgumentException(Resources.ArgumentException_CanNotReadOrSeek, nameof(contentStream));

            if (bufferSize < 1)
                throw ExceptionHelper.CanNotBeLess(nameof(bufferSize), 1);

            #endregion

            ContentStream = contentStream;
            BufferSize = bufferSize;
            InitialStreamPosition = ContentStream.Position;

            MimeContentType = "application/octet-stream";
        }


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamContent"/>.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        protected StreamContent() { }


        #region Методы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Подсчитывает и возвращает длину тела запроса в байтах.
        /// </summary>
        /// <returns>Длина контента в байтах.</returns>
        /// <exception cref="T:System.ObjectDisposedException">Текущий экземпляр уже был удалён.</exception>
        public override long CalculateContentLength()
        {
            ThrowIfDisposed();

            return ContentStream.Length;
        }

        /// <inheritdoc />
        /// <summary>
        /// Записывает данные тела запроса в поток.
        /// </summary>
        /// <param name="stream">Поток, куда будут записаны данные тела запроса.</param>
        /// <exception cref="T:System.ObjectDisposedException">Текущий экземпляр уже был удалён.</exception>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="stream" /> равно <see langword="null" />.</exception>
        public override void WriteTo(Stream stream)
        {
            ThrowIfDisposed();

            #region Проверка параметров

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            #endregion

            ContentStream.Position = InitialStreamPosition;

            var buffer = new byte[BufferSize];

            while (true)
            {
                int bytesRead = ContentStream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                stream.Write(buffer, 0, bytesRead);
            }
        }

        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Освобождает неуправляемые (а при необходимости и управляемые) ресурсы, используемые объектом <see cref="T:Leaf.xNet.HttpContent" />.
        /// </summary>
        /// <param name="disposing">Значение <see langword="true" /> позволяет освободить управляемые и неуправляемые ресурсы; значение <see langword="false" /> позволяет освободить только неуправляемые ресурсы.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing || ContentStream == null)
                return;

            ContentStream.Dispose();
            ContentStream = null;
        }


        private void ThrowIfDisposed()
        {
            if (ContentStream == null)
                throw new ObjectDisposedException("StreamContent");
        }
    }
}
