using System;
using System.Security.Cryptography;

namespace UsefulExtensions
{
    /// <summary>
    /// Класс-обёртка для потокобезопасной генерации псевдослучайных чисел.
    /// Lazy-load singleton для ThreadStatic <see cref="Random"/>.
    /// </summary>
    static class Randomizer
    {
        private static readonly RNGCryptoServiceProvider Generator = new RNGCryptoServiceProvider();

        private static Random Generate()
        {
            var buffer = new byte[4];
            Generator.GetBytes(buffer);
            return new Random(BitConverter.ToInt32(buffer, 0));
        }

        public static Random Instance => _rand ?? (_rand = Generate());
        [ThreadStatic] private static Random _rand;
    }

    /// <summary>
    /// Генератор случайных User-Agent'ов
    /// </summary>
    public class RandomUserAgentGenerator
    {
        /// <summary>
        /// Генерирует случайную версию Windows
        /// </summary>
        /// <returns>Случайная версия Windows</returns>
        private static string GenerateRandomWindowsVersion()
        {
            string windowsVersion = "Windows NT ";
            int random = Randomizer.Instance.Next(99) + 1;

            // Windows 10 = 45% popularity
            if (random >= 1 && random <= 45)
                windowsVersion += "10.0";

            // Windows 7 = 35% popularity
            else if (random > 45 && random <= 80)
                windowsVersion += "6.1";

            // Windows 8.1 = 15% popularity
            else if (random > 80 && random <= 95)
                windowsVersion += "6.3";

            // Windows 8 = 5% popularity
            else
                windowsVersion += "6.2";

            // Append WOW64 for X64 system
            if (Randomizer.Instance.NextDouble() <= 0.65)
                windowsVersion += Randomizer.Instance.NextDouble() <= 0.5 ? "; WOW64" : "; Win64; x64";

            return windowsVersion;
        }
        
        /// <summary>
        /// Генерирует случайный User-Agent от браузера Opera.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Opera.</returns>
        public static string GenerateOperaUserAgent()
        {
            string version;
            string presto;

            #region Генерация случайной версии

            switch (Randomizer.Instance.Next(4))
            {
                case 0:
                    version = "12.16";
                    presto = "2.12.388";
                    break;

                case 1:
                    version = "12.14";
                    presto = "2.12.388";
                    break;

                case 2:
                    version = "12.02";
                    presto = "2.10.289";
                    break;

                default:
                    version = "12.00";
                    presto = "2.10.181";
                    break;
            }

            #endregion

            return $"Opera/9.80 ({GenerateRandomWindowsVersion()}); U) Presto/{presto} Version/{version}";
        }

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Chrome.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Chrome.</returns>
        public static string GenerateChromeUserAgent()
        {
            int major = Randomizer.Instance.Next(62, 70);
            int build = Randomizer.Instance.Next(2100, 3538);
            int branchBuild = Randomizer.Instance.Next(170);

            return $"Mozilla/5.0 ({GenerateRandomWindowsVersion()}) AppleWebKit/537.36 (KHTML, like Gecko) " +
                $"Chrome/{major}.0.{build}.{branchBuild} Safari/537.36";
        }

        private static readonly byte[] FirefoxVersions = { 64, 63, 62, 60, 58, 52, 51, 46, 45 };

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Firefox.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Firefox.</returns>
        public static string GenerateFirefoxUserAgent()
        {
            byte version = FirefoxVersions[Randomizer.Instance.Next(FirefoxVersions.Length - 1)];

            return $"Mozilla/5.0 ({GenerateRandomWindowsVersion()}; rv:{version}.0) Gecko/20100101 Firefox/{version}.0";
        }

        /// <summary>
        /// Возвращает случайный User-Agent Chrome / Firefox / Opera, основываясь на их популярности.
        /// </summary>
        /// <returns>Строка-значение заголовка User-Agent</returns>
        public static string GenerateRandomUserAgent()
        {
            int rand = Randomizer.Instance.Next(99) + 1;

            // TODO: edge, yandex browser, safari

            // Chrome = 70%
            if (rand >= 1 && rand <= 70)
                return GenerateChromeUserAgent();

            // Firefox = 15%
            if (rand > 70 && rand <= 85)
                return GenerateFirefoxUserAgent();

            // Opera = 15%
            return GenerateOperaUserAgent();
        }
    }
}
