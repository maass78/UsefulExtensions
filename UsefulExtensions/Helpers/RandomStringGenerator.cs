using System;
using System.Text;

namespace UsefulExtensions
{
    /// <summary>
    /// Генератор случайных строк
    /// </summary>
    public class RandomStringGenerator
    {
        /// <summary>
        /// Возвращает экземпляр класса <see cref="RandomStringGenerator"/>, генерирующий строку только из цифр 0987654321
        /// </summary>
        public static RandomStringGenerator NumbersGenerator => new RandomStringGenerator("0987654321");

        /// <summary>
        /// Возвращает экземпляр класса <see cref="RandomStringGenerator"/>, генерирующий строку только из латинских букв с разным регистром и цифр
        /// </summary>
        public static RandomStringGenerator AllSymbolsGenerator => new RandomStringGenerator("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0987654321");

        /// <summary>
        /// Возвращает экземпляр класса  <see cref="RandomPasswordGenerator"/>, генерирующий строку по указанным параметрам
        /// </summary>
        /// <param name="minLength">Contains rule minimum-Length for password generate. [Default: 4]</param>
        /// <param name="maxLength">Contains rule maximum-Length for password generate. [Default: 20]</param>
        /// <param name="minLowerCase">Contains rule minimum-LowerCaseChars must included in password. [Default: 1]</param>
        /// <param name="minUpperCase">Contains rule minimum-NumericChars must included in password. [Default: 0]</param>
        /// <param name="minNumeric">Contains rule minimum-SpecialChars must included in password. [Default: 0]</param>
        /// <returns>RandomPasswordGenerator</returns>
        [Obsolete]
        public static RandomPasswordGenerator RandomPasswordAdvanced(int minLength = 4, int maxLength = 20, int minLowerCase = 1, int minUpperCase = 0, int minNumeric = 0)
        {
            return new RandomPasswordGenerator(minLength, maxLength, minLowerCase, minUpperCase, minNumeric);
        }

        /// <summary>
        /// Словарь символов, из которых генерируется случайная строка
        /// </summary>
        public string Dictionary { get; set; }

        private Random _random;

        public RandomStringGenerator(string dictionary = "ytrewqdsapoiufghjklzxcvbnm1234567890")
        {
            _random = new Random();
            Dictionary = dictionary;
        }

        /// <summary>
        /// Генерирует случайную строку с указанной длиной
        /// </summary>
        /// <param name="keyLength"></param>
        /// <returns>Случайная строка с указанной длиной, состоящая из символов словаря</returns>
        public string Generate(int keyLength)
        {
            StringBuilder sb = new StringBuilder(keyLength - 1);

            for(int i = 0; i < keyLength; i++)
                sb.Append(Dictionary[_random.Next(0, Dictionary.Length)]);

            return sb.ToString();
        }
    }
}