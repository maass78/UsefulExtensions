using System.IO;
using System.Security.Cryptography;
using UsefulExtensions.Security.Types;

namespace UsefulExtensions.Security
{
    /// <summary>
    /// Класс-обертка для реализаций <see cref="SymmetricAlgorithm"/>
    /// </summary>
    /// <typeparam name="T">Одна из реализаций <see cref="SymmetricAlgorithm"/>, используемая для шифрования</typeparam>
    public class SymmetricCipher<T> where T : SymmetricAlgorithm, new()
    {
        /// <summary>
        /// Параметры генерации ключа
        /// </summary>
        public SymmetricCipherParams Parameters { get; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <param name="parameters">Параметры генерации ключа</param>
        public SymmetricCipher(SymmetricCipherParams parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Шифрует массив байтов
        /// </summary>
        /// <param name="value">Массив байтов, которые необходимо зашифровать</param>
        /// <returns>Зашифрованный массив байтов</returns>
        public byte[] Encrypt(byte[] value)
        {
            byte[] encrypted;
            using (T cipher = new T())
            {
                cipher.Mode = CipherMode.CBC;
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(Parameters.Password, Parameters.Salt, Parameters.HashAlgorithm.ToString(), Parameters.Iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(Parameters.KeySize / 8);

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, Parameters.IV))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(value, 0, value.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return encrypted;
        }

        /// <summary>
        /// Расшифровывает массив байтов
        /// </summary>
        /// <param name="value">Массив зашифрованных байтов</param>
        /// <returns>Расшифрованный массив байтов</returns>
        public byte[] Decrypt(byte[] value)
        {
            byte[] decrypted;
            using (T cipher = new T())
            {
                cipher.Mode = CipherMode.CBC;
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(Parameters.Password, Parameters.Salt, Parameters.HashAlgorithm.ToString(), Parameters.Iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(Parameters.KeySize / 8);
                
                using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, Parameters.IV))
                {
                    using (MemoryStream from = new MemoryStream(value))
                    {
                        using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                        {
                            decrypted = new byte[value.Length];
                            reader.Read(decrypted, 0, decrypted.Length);
                        }
                    }
                }
                cipher.Clear();
            }
            return decrypted;
        }
    }
}
