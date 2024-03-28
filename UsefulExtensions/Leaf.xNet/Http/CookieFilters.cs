using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Leaf.xNet
{
    public static class CookieFilters
    {
        public static bool Enabled { get; set; } = true;
        
        public static bool Trim { get; set; } = true;
        public static bool Path { get; set; } = true;
        public static bool CommaEndingValue { get; set; } = true;

        /// <summary>
        /// Фильтруем Cookie для дальнейшего использования в нативном хранилище.
        /// </summary>
        /// <param name="rawCookie">Запись Cookie как строка со всеми параметрами</param>
        /// <returns>Отфильтрованная Cookie в виде строки со всеми отфильтрованными параметрами</returns>
        public static string Filter(string rawCookie)
        {
            return !Enabled ? rawCookie
                : rawCookie
                    .TrimWhitespace()
                    .FilterPath()
                    .FilterInvalidExpireYear()
                    .FilterCommaEndingValue();
        }

        /// <summary>
        /// Фильтр неверных доменов перед помещением <see cref="System.Net.Cookie"/> в <see cref="CookieStorage"/>.
        /// </summary>
        /// <param name="domain">Домен куки из заголовка domain</param>
        /// <returns>Вернет <see langword="null"/> если домен не является корректным для помещения в хранилище <see cref="CookieStorage"/></returns>
        public static string FilterDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return null;

            domain = domain.Trim('\t', '\n', '\r', ' ');
            bool isWildCard = domain.Length > 1 && domain[0] == '.';
            bool isFirstLevel = domain.IndexOf('.', 1) == -1;

            // Local wildcard domains aren't accepted by CookieStorage and native CookieContainer.
            return isWildCard && isFirstLevel ? domain.Substring(1) : domain;
        }

        /// <summary>Убираем любые пробелы в начале и конце</summary>
        private static string TrimWhitespace(this string rawCookie)
        {
            return !Trim ? rawCookie : rawCookie.Trim();
        }

        /// <summary>Заменяем все значения path на "/"</summary>
        private static string FilterPath(this string rawCookie)
        {
            if (!Path)
                return rawCookie;

            const string path = "path=/";
            int pathIndex = rawCookie.IndexOf(path, 0, StringComparison.OrdinalIgnoreCase);
            if (pathIndex == -1)
                return rawCookie;

            pathIndex += path.Length;
            if (pathIndex >= rawCookie.Length - 1 || rawCookie[pathIndex] == ';')
                return rawCookie;

            int endPathIndex = rawCookie.IndexOf(';', pathIndex);
            if (endPathIndex == -1)
                endPathIndex = rawCookie.Length;

            return rawCookie.Remove(pathIndex, endPathIndex - pathIndex);
        }


        /// <summary>Заменяем значения кук завершающиеся запятой (escape)</summary>
        private static string FilterCommaEndingValue(this string rawCookie)
        {
            if (!CommaEndingValue)
                return rawCookie;

            int equalIndex = rawCookie.IndexOf('=');
            if (equalIndex == -1 || equalIndex >= rawCookie.Length - 1)
                return rawCookie;

            int endValueIndex = rawCookie.IndexOf(';', equalIndex + 1);
            if (endValueIndex == -1)
                endValueIndex = rawCookie.Length - 1;

            int lastCharIndex = endValueIndex - 1;
            return rawCookie[lastCharIndex] != ','
                ? rawCookie
                : rawCookie.Remove(lastCharIndex, 1).Insert(lastCharIndex, "%2C");
        }

        /// <summary>
        /// Исправляет исключение при GMT 9999 года методом замены на 9998 год.
        /// </summary>
        /// <returns>Вернет исправленную куку с годом 9998 вместо 9999 при котором может возникнуть исключение.</returns>
        private static string FilterInvalidExpireYear(this string rawCookie)
        {
            const string expireKey = "expires=";
            const string invalidYear = "9999";

            int startIndex = rawCookie.IndexOf(expireKey, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1)
                return rawCookie;
            startIndex += expireKey.Length;

            int endIndex = rawCookie.IndexOf(';', startIndex);
            if (endIndex == -1)
                endIndex = rawCookie.Length;

            string expired = rawCookie.Substring(startIndex, endIndex - startIndex);

            int invalidYearIndex = expired.IndexOf(invalidYear, StringComparison.Ordinal);
            if (invalidYearIndex == -1)
                return rawCookie;
            invalidYearIndex += startIndex + invalidYear.Length - 1;

            return rawCookie.Remove(invalidYearIndex, 1).Insert(invalidYearIndex, "8");
        }
    }
}