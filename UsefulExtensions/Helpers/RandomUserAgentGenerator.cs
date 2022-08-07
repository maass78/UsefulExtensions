using System;

namespace UsefulExtensions
{
    /// <summary>
    /// Генератор случайных User-Agent'ов
    /// </summary>
    public class RandomUserAgentGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Генерирует случайную версию Windows
        /// </summary>
        /// <returns>Случайная версия Windows</returns>
        private static string GenerateRandomWindowsVersion()
        {
            string windowsVersion = "Windows NT ";
            int random = _random.Next(99) + 1;

            // Windows 10 = 45% popularity
            if(random >= 1 && random <= 45)
                windowsVersion += "10.0";

            // Windows 7 = 35% popularity
            else if(random > 45 && random <= 80)
                windowsVersion += "6.1";

            // Windows 8.1 = 15% popularity
            else if(random > 80 && random <= 95)
                windowsVersion += "6.3";

            // Windows 8 = 5% popularity
            else
                windowsVersion += "6.2";

            // Append WOW64 for X64 system
            if(_random.NextDouble() <= 0.65)
                windowsVersion += _random.NextDouble() <= 0.5 ? "; WOW64" : "; Win64; x64";

            return windowsVersion;
        }

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Chrome.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Chrome.</returns>
        public static string GenerateChromeUserAgent()
        {
            int major = _random.Next(90, 104);
            int build = _random.Next(4430, 5006);
            int branchBuild = _random.Next(170);

            return $"Mozilla/5.0 ({GenerateRandomWindowsVersion()}) AppleWebKit/537.36 (KHTML, like Gecko) " +
                $"Chrome/{major}.0.{build}.{branchBuild} Safari/537.36";
        }

        private static readonly byte[] FirefoxVersions = { 89, 91, 99, 98, 97, 96, 95, 94, 93, 92, 100 };

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Firefox.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Firefox.</returns>
        public static string GenerateFirefoxUserAgent()
        {
            byte version = FirefoxVersions[_random.Next(FirefoxVersions.Length - 1)];

            return $"Mozilla/5.0 ({GenerateRandomWindowsVersion()}; rv:{version}.0) Gecko/20100101 Firefox/{version}.0";
        }

        /// <summary>
        /// Генерирует случайный User-Agent от Apple iPhone.
        /// </summary>
        /// <returns>Случайный User-Agent от  Apple iPhone.</returns>
        public static string GenerateMobileUserAgent()
        {
            int major = _random.Next(10, 13);
            int build = _random.Next(1, 3);
            int branchBuild = build + 2;

            return $"Mozilla/5.0 ({GenerateRandomWindowsVersion()}) AppleWebKit/605.1.15 (KHTML, like Gecko) " +
                   $"Version/{major}.1.{build} Mobile/{branchBuild}E148 Safari/604.1";
        }

        /// <summary>
        /// Возвращает случайный User-Agent Chrome / Firefox / Opera, основываясь на их популярности.
        /// </summary>
        /// <returns>Строка-значение заголовка User-Agent</returns>
        public static string GenerateRandomUserAgent()
        {
            int rand = _random.Next(99) + 1;

            // TODO: edge, yandex browser, safari

            // Chrome = 70%
            if(rand >= 1 && rand <= 70)
                return GenerateChromeUserAgent();

            // Apple iPhone = 25% popularity
            else if(rand >= 55 && rand <= 80)
                return GenerateMobileUserAgent();
            else
                return GenerateFirefoxUserAgent();
        }
    }
}