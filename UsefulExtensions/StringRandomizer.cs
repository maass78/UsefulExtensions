using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace UsefulExtensions
{
    /// <summary>
    /// Предоставляет методы для выбора рандомного варианта строки
    /// </summary>
    /// <remarks>
    /// Пример кода для данного класса: <para/>
    /// <code>
    /// var randomizer = new StringRandomaizer("this string is {nice|ugly}");<para/>
    /// string randomString = randomizer.Get();
    /// </code>
    /// На выходе:<para/>
    /// <list type="bullet">
    ///     <item>
    ///         <term>this string is nice</term>
    ///         <description>Первый вариант</description>
    ///     </item>
    ///     <item>
    ///         <term>this string is ugly</term>
    ///         <description>Второй вариант</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public class StringRandomizer
    {
        private string _source;
        private Random _random;

        /// <summary>
        /// Конструктор класса <see cref="StringRandomizer"/>
        /// </summary>
        /// <param name="source">Входная строка (формат см. в описании класса)</param>
        public StringRandomizer(string source)
        {
            _random = new Random();
            _source = source;
        }

        /// <summary>
        /// Возвращает рандомизированную строку (пример кода см. в описании класса)
        /// </summary>
        /// <returns>Рандомизированная строка</returns>
        public string Get()
        {
            var matches = Regex.Matches(_source, @"{(.*?)\|(.*?)}").Cast<Match>();

            string result = _source;

            foreach (var match in matches)
            {
                result = result.Replace(match.Value, match.Groups[_random.Next() % 2 == 0 ? 1 : 2].Value);
            }

            return result;
        }

    }
}
