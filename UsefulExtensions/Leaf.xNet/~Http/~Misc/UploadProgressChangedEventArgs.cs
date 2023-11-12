using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет данные для события, сообщающим о прогрессе выгрузки данных.
    /// </summary>
    public sealed class UploadProgressChangedEventArgs : EventArgs
    {
        #region Свойства (открытые)

        /// <summary>
        /// Возвращает количество отправленных байтов.
        /// </summary>
        public long BytesSent { get; }

        /// <summary>
        /// Возвращает общее количество отправляемых байтов.
        /// </summary>
        public long TotalBytesToSend { get; }

        /// <summary>
        /// Возвращает процент отправленных байтов.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public double ProgressPercentage => (double)BytesSent / TotalBytesToSend * 100.0;

        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.UploadProgressChangedEventArgs" />.
        /// </summary>
        /// <param name="bytesSent">Количество отправленных байтов.</param>
        /// <param name="totalBytesToSend">Общее количество отправляемых байтов.</param>
        public UploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend)
        {
            BytesSent = bytesSent;
            TotalBytesToSend = totalBytesToSend;
        }
    }
}
