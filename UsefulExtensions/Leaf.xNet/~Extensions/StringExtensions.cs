using System;
using System.Collections.Generic;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Исключение говорящее о том что не удалось найти одну или несколько подстрок между двумя подстроками.
    /// </summary>
    public class SubstringException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Исключение говорящее о том что не удалось найти одну или несколько подстрок между двумя подстроками.
        /// </summary>
        public SubstringException() { }

        /// <inheritdoc />
        /// <inheritdoc cref="SubstringException()"/>        
        public SubstringException(string message) : base(message) {}

        /// <inheritdoc />
        /// <inheritdoc cref="SubstringException()"/>
        public SubstringException(string message, Exception innerException) : base(message, innerException) {}
    }

    /// <summary>
    /// Этот класс является расширением для строк. Не нужно его вызывать напрямую.
    /// </summary>
    public static class StringExtensions
    {
        #region Substrings: Несколько строк

        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, вернет пустой массив.
        /// </summary>
        /// <param name="self">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparison">Метод сравнения строк</param>
        /// <param name="limit">Максимальное число подстрок для поиска</param>
        /// <exception cref="ArgumentNullException">Возникает если один из параметров пустая строка или <keyword>null</keyword>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Возникает если начальный индекс превышает длину строки.</exception>
        /// <returns>Возвращает массив подстрок которые попадают под шаблон или пустой массив если нет совпадений.</returns>
        public static string[] SubstringsOrEmpty(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            #region Проверка параметров
            if (string.IsNullOrEmpty(self))
                return new string[0];

            if (string.IsNullOrEmpty(left))
                throw new ArgumentNullException(nameof(left));

            if (string.IsNullOrEmpty(right))
                throw new ArgumentNullException(nameof(right));

            if (startIndex < 0 || startIndex >= self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            #endregion

            int currentStartIndex = startIndex;
            int current = limit;
            var strings = new List<string>();

            while (true)
            {
                if (limit > 0)
                {
                    --current;
                    if (current < 0)
                        break;
                }

                // Ищем начало позиции левой подстроки.
                int leftPosBegin = self.IndexOf(left, currentStartIndex, comparison);
                if (leftPosBegin == -1)
                    break;

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;
                // Ищем начало позиции правой строки.
                int rightPos = self.IndexOf(right, leftPosEnd, comparison);
                if (rightPos == -1)
                    break;

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;
                strings.Add(self.Substring(leftPosEnd, length));
                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }
            
            return strings.ToArray();
        }


        /// <inheritdoc cref="SubstringsOrEmpty"/>
        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, вернет <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.        
        /// </remarks>
        /// <example>
        /// str.Substrings("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="SubstringsEx"/> - которая и так бросает исключение <see cref="SubstringException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        /// <param name="fallback">Значение в случае если подстроки не найдены</param>
        /// <returns>Возвращает массив подстрок которые попадают под шаблон или <keyword>null</keyword>.</returns>
        public static string[] Substrings(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0, string[] fallback = null)
        {
            var result = SubstringsOrEmpty(self, left, right, startIndex, comparison, limit);

            return result.Length > 0 ? result : fallback;
        }


        /// <inheritdoc cref="SubstringsOrEmpty"/>
        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, будет брошено исключение <see cref="SubstringException"/>.
        /// </summary>
        /// <exception cref="SubstringException">Будет брошено если совпадений не было найдено</exception>
        /// <returns>Возвращает массив подстрок которые попадают под шаблон или бросает исключение <see cref="SubstringException"/> если совпадений не было найдено.</returns>
        public static string[] SubstringsEx(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            var result = SubstringsOrEmpty(self, left, right, startIndex, comparison, limit);
            if (result.Length == 0)
                throw new SubstringException($"Substrings not found. Left: \"{left}\". Right: \"{right}\".");

            return result;
        }

        #endregion


        #region Substring: Одна подстрока. Прямой порядок (слева направо)

        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, вернет <paramref name="fallback"/> или по-умолчанию <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.</remarks>
        /// <example>
        /// str.Between("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="SubstringEx"/> - которая и так бросает исключение <see cref="SubstringException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        /// <param name="self">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparison">Метод сравнения строк</param>
        /// <param name="fallback">Значение в случае если подстрока не найдена</param>
        /// <returns>Возвращает строку между двумя подстроками или <paramref name="fallback"/> (по-умолчанию <keyword>null</keyword>).</returns>
        public static string Substring(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string fallback = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) ||
                startIndex < 0 || startIndex >= self.Length)
                return fallback;

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = self.IndexOf(left, startIndex, comparison);
            if (leftPosBegin == -1)
                return fallback;

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;
            // Ищем начало позиции правой строки.
            int rightPos = self.IndexOf(right, leftPosEnd, comparison);

            return rightPos != -1 ? self.Substring(leftPosEnd, rightPos - leftPosEnd) : fallback;
        }


        /// <inheritdoc cref="Substring"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, вернет пустую строку.
        /// </summary>
        /// <returns>Возвращает строку между двумя подстроками. Если совпадений нет, вернет пустую строку.</returns>
        public static string SubstringOrEmpty(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return Substring(self, left, right, startIndex, comparison, string.Empty);
        }

        /// <inheritdoc cref="Substring"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, будет брошено исключение <see cref="SubstringException"/>.
        /// </summary>
        /// <exception cref="SubstringException">Будет брошено если совпадений не было найдено</exception>
        /// <returns>Возвращает строку между двумя подстроками или бросает исключение <see cref="SubstringException"/> если совпадений не было найдено.</returns>
        public static string SubstringEx(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return Substring(self, left, right, startIndex, comparison)
                ?? throw new SubstringException($"Substring not found. Left: \"{left}\". Right: \"{right}\".");
        }


        #endregion


        #region Вырезание одной подстроки. Обратный порядок (справа налево)
        
        /// <inheritdoc cref="Substring"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, вернет <paramref name="notFoundValue"/> или по-умолчанию <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.</remarks>
        /// <example>
        /// str.BetweenLast("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="SubstringLastEx"/> - которая и так бросает исключение <see cref="SubstringException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        public static string SubstringLast(this string self, string right, string left,
            int startIndex = -1, StringComparison comparison = StringComparison.Ordinal,
            string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) ||
                startIndex < -1 || startIndex >= self.Length)
                return notFoundValue;

            if (startIndex == -1)
                startIndex = self.Length - 1;

            // Ищем начало позиции правой подстроки с конца строки
            int rightPosBegin = self.LastIndexOf(right, startIndex, comparison);
            if (rightPosBegin == -1 || rightPosBegin == 0) // в обратном поиске имеет смысл проверять на 0
                return notFoundValue;

            // Вычисляем начало позиции левой подстроки
            int leftPosBegin = self.LastIndexOf(left, rightPosBegin - 1, comparison);
            // Если не найден левый конец или правая и левая подстрока склеены вместе - вернем пустую строку
            if (leftPosBegin == -1 || rightPosBegin - leftPosBegin == 1)
                return notFoundValue;

            int leftPosEnd = leftPosBegin + left.Length;
            return self.Substring(leftPosEnd, rightPosBegin - leftPosEnd);
        }


        /// <inheritdoc cref="SubstringOrEmpty"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, вернет пустую строку.
        /// </summary>
        public static string SubstringLastOrEmpty(this string self, string right, string left,
            int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
        {
            return SubstringLast(self, right, left, startIndex, comparison, string.Empty);
        }
        
        /// <inheritdoc cref="SubstringEx"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, будет брошено исключение <see cref="SubstringException"/>.
        /// </summary>
        public static string SubstringLastEx(this string self, string right, string left,
            int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
        {
            return SubstringLast(self, right, left, startIndex, comparison)
                ?? throw new SubstringException($"StringBetween not found. Right: \"{right}\". Left: \"{left}\".");
        }

        #endregion


        #region Дополнительные функции

        /// <summary>
        /// Проверяет наличие подстроки в строке, без учета реестра, через сравнение: <see cref="StringComparison.OrdinalIgnoreCase" />.
        /// </summary>
        /// <param name="self">Строка</param>
        /// <param name="value">Подстрока которую следует искать в исходной строке</param>
        /// <returns>Вернет <langword>true</langword> </returns>
        public static bool ContainsInsensitive(this string self, string value)
        {
            return self.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        #endregion
    }
}
