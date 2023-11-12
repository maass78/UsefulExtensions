using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
#if IS_NETFRAMEWORK
using System.Web;
// ReSharper disable MemberCanBePrivate.Global
#endif

namespace Leaf.xNet
{
    /// <summary>
    /// Представляет статический класс, предназначенный для помощи в работе с HTTP-протоколом.
    /// </summary>
    public static class Http
    {
        #region Константы (открытые)

        /// <summary>
        /// Обозначает новую строку в HTTP-протоколе.
        /// </summary>
        public const string NewLine = "\r\n";

        /// <summary>
        /// Метод делегата, который принимает все сертификаты SSL.
        /// </summary>
        public static readonly RemoteCertificateValidationCallback AcceptAllCertificationsCallback;

        #endregion

        #region Статические поля (внутренние)

        internal static readonly Dictionary<HttpHeader, string> Headers = new Dictionary<HttpHeader, string>
        {
            { HttpHeader.Accept, "Accept" },
            { HttpHeader.AcceptCharset, "Accept-Charset" },
            { HttpHeader.AcceptLanguage, "Accept-Language" },
            { HttpHeader.AcceptDatetime, "Accept-Datetime" },
            { HttpHeader.CacheControl, "Cache-Control" },
            { HttpHeader.ContentType, "Content-Type" },
            { HttpHeader.Date, "Date" },
            { HttpHeader.Expect, "Expect" },
            { HttpHeader.From, "From" },
            { HttpHeader.IfMatch, "If-Match" },
            { HttpHeader.IfModifiedSince, "If-Modified-Since" },
            { HttpHeader.IfNoneMatch, "If-None-Match" },
            { HttpHeader.IfRange, "If-Range" },
            { HttpHeader.IfUnmodifiedSince, "If-Unmodified-Since" },
            { HttpHeader.MaxForwards, "Max-Forwards" },
            { HttpHeader.Pragma, "Pragma" },
            { HttpHeader.Range, "Range" },
            { HttpHeader.Referer, "Referer" },
            { HttpHeader.Origin, "Origin" },
            { HttpHeader.Upgrade, "Upgrade" },
            { HttpHeader.UpgradeInsecureRequests, "Upgrade-Insecure-Requests"},
            { HttpHeader.UserAgent, "User-Agent" },
            { HttpHeader.Via, "Via" },
            { HttpHeader.Warning, "Warning" },
            { HttpHeader.DNT, "DNT" },
            { HttpHeader.AccessControlAllowOrigin, "Access-Control-Allow-Origin" },
            { HttpHeader.AcceptRanges, "Accept-Ranges" },
            { HttpHeader.Age, "Age" },
            { HttpHeader.Allow, "Allow" },
            { HttpHeader.ContentEncoding, "Content-Encoding" },
            { HttpHeader.ContentLanguage, "Content-Language" },
            { HttpHeader.ContentLength, "Content-Length" },
            { HttpHeader.ContentLocation, "Content-Location" },
            { HttpHeader.ContentMD5, "Content-MD5" },
            { HttpHeader.ContentDisposition, "Content-Disposition" },
            { HttpHeader.ContentRange, "Content-Range" },
            { HttpHeader.ETag, "ETag" },
            { HttpHeader.Expires, "Expires" },
            { HttpHeader.LastModified, "Last-Modified" },
            { HttpHeader.Link, "Link" },
            { HttpHeader.Location, "Location" },
            { HttpHeader.P3P, "P3P" },
            { HttpHeader.Refresh, "Refresh" },
            { HttpHeader.RetryAfter, "Retry-After" },
            { HttpHeader.Server, "Server" },
            { HttpHeader.TransferEncoding, "Transfer-Encoding" }
        };

        #endregion

        static Http() => AcceptAllCertificationsCallback = AcceptAllCertifications;

        #region Статические методы (открытые)

        /// <summary>
        /// Преобразует параметры в строку запроса.
        /// </summary>
        /// <param name="parameters">Параметры.</param>
        /// <param name="valuesUnescaped">Указывает, нужно ли пропустить кодирование значений параметров запроса.</param>
        /// <param name="keysUnescaped">Указывает, нужно ли пропустить кодирование имен параметров запроса.</param>
        /// <returns>Строка запроса.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="parameters"/> равно <see langword="null"/>.</exception>
        // ReSharper disable once UnusedMember.Global
        public static string ToQueryString(IEnumerable<KeyValuePair<string, string>> parameters, 
            bool valuesUnescaped = false, bool keysUnescaped = false)
        {
            #region Проверка параметров

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            #endregion

            var queryBuilder = new StringBuilder();

            foreach (var param in parameters)
            {
                if (string.IsNullOrEmpty(param.Key))
                    continue;

                queryBuilder.Append(keysUnescaped ? param.Key : Uri.EscapeDataString(param.Key));
                queryBuilder.Append('=');

                queryBuilder.Append(valuesUnescaped ? param.Value : Uri.EscapeDataString(param.Value ?? string.Empty));

                queryBuilder.Append('&');
            }

            // Удаляем '&' в конце, если есть контент
            if (queryBuilder.Length != 0)
                queryBuilder.Remove(queryBuilder.Length - 1, 1);

            return queryBuilder.ToString();
        }

        /// <summary>
        /// Определяет и возвращает MIME-тип на основе расширения файла.
        /// </summary>
        /// <param name="extension">Расширение файла.</param>
        /// <returns>MIME-тип.</returns>
        public static string DetermineMediaType(string extension)
        {
            #if IS_NETFRAMEWORK
            return MimeMapping.GetMimeMapping(extension);
            #else
            if (NetStandard.MimeTypes.ContainsKey(extension))
                return NetStandard.MimeTypes[extension];

            return "application/octet-stream";
            #endif
        }

        #region User Agent

        /// <summary>
        /// Генерирует случайный User-Agent от браузера IE.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера IE.</returns>
        public static string IEUserAgent()
        {
            string windowsVersion = RandomWindowsVersion();

            string version;
            string mozillaVersion;
            string trident;
            string otherParams;

            #region Генерация случайной версии

            if (windowsVersion.Contains("NT 5.1"))
            {
                version = "9.0";
                mozillaVersion = "5.0";
                trident = "5.0";
                otherParams = ".NET CLR 2.0.50727; .NET CLR 3.5.30729";
            }
            else if (windowsVersion.Contains("NT 6.0"))
            {
                version = "9.0";
                mozillaVersion = "5.0";
                trident = "5.0";
                otherParams = ".NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729";
            }
            else
            {
                switch (Randomizer.Instance.Next(3))
                {
                    case 0:
                        version = "10.0";
                        trident = "6.0";
                        mozillaVersion = "5.0";
                        break;

                    case 1:
                        version = "10.6";
                        trident = "6.0";
                        mozillaVersion = "5.0";
                        break;

                    default:
                        version = "11.0";
                        trident = "7.0";
                        mozillaVersion = "5.0";
                        break;
                }

                otherParams = ".NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
            }

            #endregion

            return
                $"Mozilla/{mozillaVersion} (compatible; MSIE {version}; {windowsVersion}; Trident/{trident}; {otherParams})";
        }

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Opera.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Opera.</returns>
        public static string OperaUserAgent()
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

            return $"Opera/9.80 ({RandomWindowsVersion()}); U) Presto/{presto} Version/{version}";
        }

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Chrome.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Chrome.</returns>
        public static string ChromeUserAgent()
        {
            int major = Randomizer.Instance.Next(62, 70);
            int build = Randomizer.Instance.Next(2100, 3538);
            int branchBuild = Randomizer.Instance.Next(170);

            return $"Mozilla/5.0 ({RandomWindowsVersion()}) AppleWebKit/537.36 (KHTML, like Gecko) " +
                $"Chrome/{major}.0.{build}.{branchBuild} Safari/537.36";
        }


        private static readonly byte[] FirefoxVersions = { 64, 63, 62, 60, 58, 52, 51, 46, 45 };

        /// <summary>
        /// Генерирует случайный User-Agent от браузера Firefox.
        /// </summary>
        /// <returns>Случайный User-Agent от браузера Firefox.</returns>
        public static string FirefoxUserAgent()
        {
            byte version = FirefoxVersions[Randomizer.Instance.Next(FirefoxVersions.Length - 1)];

            return $"Mozilla/5.0 ({RandomWindowsVersion()}; rv:{version}.0) Gecko/20100101 Firefox/{version}.0";
        }

        /// <summary>
        /// Генерирует случайный User-Agent от мобильного браузера Opera.
        /// </summary>
        /// <returns>Случайный User-Agent от мобильного браузера Opera.</returns>
        public static string OperaMiniUserAgent()
        {
            string os;
            string miniVersion;
            string version;
            string presto;

            #region Генерация случайной версии

            switch (Randomizer.Instance.Next(3))
            {
                case 0:
                    os = "iOS";
                    miniVersion = "7.0.73345";
                    version = "11.62";
                    presto = "2.10.229";
                    break;

                case 1:
                    os = "J2ME/MIDP";
                    miniVersion = "7.1.23511";
                    version = "12.00";
                    presto = "2.10.181";
                    break;

                default:
                    os = "Android";
                    miniVersion = "7.5.54678";
                    version = "12.02";
                    presto = "2.10.289";
                    break;
            }

            #endregion

            return $"Opera/9.80 ({os}; Opera Mini/{miniVersion}/28.2555; U; ru) Presto/{presto} Version/{version}";
        }

        /// <summary>
        /// Возвращает случайный User-Agent Chrome / Firefox / Opera основываясь на их популярности.
        /// </summary>
        /// <returns>Строка-значение заголовка User-Agent</returns>
        public static string RandomUserAgent()
        {
            int rand = Randomizer.Instance.Next(99) + 1;

            // TODO: edge, yandex browser, safari

            // Chrome = 70%
            if (rand >= 1 && rand <= 70)
                return ChromeUserAgent();

            // Firefox = 15%
            if (rand > 70 && rand <= 85)
                return FirefoxUserAgent();

            // IE = 6%
            if (rand > 85 && rand <= 91)
                return IEUserAgent();

            // Opera 12 = 5%
            if (rand > 91 && rand <= 96)
                return OperaUserAgent();

            // Opera mini = 4%
            return OperaMiniUserAgent();
        }

        #endregion

        #endregion


        #region Статические методы (закрытые)

        private static bool AcceptAllCertifications(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            SslPolicyErrors sslPolicyErrors) => true;

        private static string RandomWindowsVersion()
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

        #endregion
    }
}
