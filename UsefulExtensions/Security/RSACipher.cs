using System;
using System.Security.Cryptography;

namespace UsefulExtensions.Security
{
    /// <summary>
    /// Класс-обертка над <see cref="RSACryptoServiceProvider"/>
    /// </summary>
    public class RSACipher : IDisposable
    {
        public CspParameters CspParams { get; }
        public RSACryptoServiceProvider RSA { get; }

        /// <summary>
        /// Конструктор класса <see cref="RSACipher"/>
        /// </summary>
        public RSACipher()
        {
            CspParams = new CspParameters();
            RSA = new RSACryptoServiceProvider(CspParams)
            {
                PersistKeyInCsp = true
            };
        }

        /// <summary>
        /// Возвращает информацию о ключе без приватных данных
        /// </summary>
        /// <returns>CspBlob без приватной информации</returns>
        public byte[] GetPublicCsp() => RSA.ExportCspBlob(false);

        /// <summary>
        /// Устанавливает данные CspBlob для объекта <see cref="RSA"/>
        /// </summary>
        /// <param name="cspBlob">CspBlob, который необходимо установить</param>
        public void SetCsp(byte[] cspBlob) => RSA.ImportCspBlob(cspBlob);

        /// <summary>
        /// Освобождает все ресурсы, используемые данным объектом
        /// </summary>
        public void Dispose()
        {
            RSA.Clear();
            RSA.Dispose();
        }

        /// <summary>
        /// Шифрует массив байтов
        /// </summary>
        /// <param name="value">Массив байтов, которые необходимо зашифровать</param>
        /// <returns>Зашифрованный массив байтов</returns>
        public byte[] Encrypt(byte[] value) => RSA.Encrypt(value, false);

        /// <summary>
        /// Расшифровывает массив байтов
        /// </summary>
        /// <param name="value">Массив зашифрованных байтов</param>
        /// <returns>Расшифрованный массив байтов</returns>
        public byte[] Decrypt(byte[] value) => RSA.Decrypt(value, false);
    } 
}
