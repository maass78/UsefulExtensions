using System;
using System.Text;

namespace UsefulExtensions
{
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

        public string Dictionary { get; set; }
        private Random _random;

        public RandomStringGenerator(string dictionary = "ytrewqdsapoiufghjklzxcvbnm1234567890")
        {
            _random = new Random();
            Dictionary = dictionary;
        }

        public string Generate(int keyLenght)
        {
            StringBuilder sb = new StringBuilder(keyLenght - 1);

            for (int i = 0; i < keyLenght; i++)
                sb.Append(Dictionary[_random.Next(0, Dictionary.Length)]);

            return sb.ToString();
        }
    }
}
