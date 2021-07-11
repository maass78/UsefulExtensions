using System.Security.Cryptography;

namespace UsefulExtensions.Security.Utilities
{
    /// <summary>
    /// Статический класс, предоставляющий методы для работы с ключами шифрования
    /// </summary>
    public static class Cryptography
    {
        /// <summary>
        /// Возвращает массив байтов, наполненных случайными значениями
        /// </summary>
        /// <param name="bufferSize">Количество байтов</param>
        /// <returns>Массив байтов, наполненных случайными значениями</returns>
        public static byte[] GenerateRandomBytes(int bufferSize)
        {
            byte[] bytes = new byte[bufferSize];
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
