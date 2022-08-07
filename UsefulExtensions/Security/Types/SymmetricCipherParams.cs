using UsefulExtensions.Security.Utilities;

namespace UsefulExtensions.Security.Types
{
    public enum HashAlgorithm
    {
        SHA1,
        MD5
    }

    /// <summary>
    /// Информация о ключе симметричного шифрования
    /// </summary>
    public class SymmetricCipherParams
    {
        /// <summary>
        /// Конструктор класса <see cref="SymmetricCipherParams"/>
        /// </summary>
        /// <param name="password">Пароль для генерации ключа</param>
        public SymmetricCipherParams(string password)
        {
            Password = password;
            Salt = Cryptography.GenerateRandomBytes(16);
            IV = Cryptography.GenerateRandomBytes(16);
            HashAlgorithm = HashAlgorithm.SHA1;
            Iterations = 2;
            KeySize = 256;
        }

        /// <summary>
        /// Конструктор класса <see cref="SymmetricCipherParams"/>
        /// </summary>
        /// <param name="password">Пароль для генерации ключа</param>
        /// <param name="salt">Соль</param>
        /// <param name="iv">Вектор инициализации</param>
        public SymmetricCipherParams(string password, byte[] salt, byte[] iv)
        {
            Password = password;
            Salt = salt;
            IV = iv;
            HashAlgorithm = HashAlgorithm.SHA1;
            Iterations = 2;
            KeySize = 256;
        }

        /// <summary>
        /// Конструктор класса <see cref="SymmetricCipherParams"/> по умолчанию
        /// </summary>
        public SymmetricCipherParams()
        {
        }

        /// <summary>
        /// Пароль, используемый для генерации ключа
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Массив байтов, используемый для "соления" пароля
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// Вектор инициализации
        /// </summary>
        public byte[] IV { get; set; }

        /// <summary>
        /// Метод хеширования пароля
        /// </summary>
        public HashAlgorithm HashAlgorithm { get; set; }

        /// <summary>
        /// Количество итераций генерации ключа
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Размер ключа в битах
        /// </summary>
        public int KeySize { get; set; }
    }
}