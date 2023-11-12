using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Leaf.xNet
{
    [Serializable]
    public class CookieStorage
    {
        /// <summary>
        /// Оригинальный Cookie контейнер <see cref="CookieContainer"/> из .NET Framework.
        /// </summary>
        public CookieContainer Container { get; private set; }

        /// <summary>
        /// Число <see cref="Cookie"/> в <see cref="CookieContainer"/> (для всех адресов).
        /// </summary>
        public int Count => Container.Count;

        /// <summary>
        /// Возвращает или задает значение, указывающие, закрыты ли куки для редактирования через ответы сервера.
        /// </summary>
        /// <value>Значение по умолчанию — <see langword="false"/>.</value>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Значение по умолчанию для всех экземпляров.
        /// Сбрасывать старую Cookie при вызове <see cref="Set(Cookie)"/> если найдено совпадение по домену и имени Cookie.
        /// </summary>
        public static bool DefaultExpireBeforeSet { get; set; } = true;

        /// <summary>        
        /// Сбрасывать старую Cookie при вызове <see cref="Set(Cookie)"/> если найдено совпадение по домену и имени Cookie.
        /// </summary>
        public bool ExpireBeforeSet { get; set; } = DefaultExpireBeforeSet;

        /// <summary>
        /// Возвращает или задаёт экранирование символов значения Cookie получаемого от сервера.
        /// </summary>
        public bool EscapeValuesOnReceive { get; set; } = true;

        /// <summary>
        /// Dont throw exception when received cookie name is invalid, just ignore.
        /// </summary>
        public bool IgnoreInvalidCookie { get; set; }
      
        /// <summary>
        /// Пропускать куки которые истекли в ответе. Если указать <see langword="true" /> (по умолчанию), истекшее значение Cookie не будет обновляться и удаляться. 
        /// </summary>
        public bool IgnoreSetForExpiredCookies { get; set; } = true;

        /// <summary>
        /// Возвращает или задаёт возможность де-экранировать символы значения Cookie прежде чем отправлять запрос на сервер.
        /// <remarks>
        /// По умолчанию задан тому же значению что и <see cref="EscapeValuesOnReceive"/>.
        /// Иными словами, по умолчанию режим работы такой: получили - экранировали значение в хранилище, отправляем - де-экранируем значение и отправляем на сервер оригинальное.
        /// </remarks>
        /// </summary>
        public bool UnescapeValuesOnSend {
            get => !_unescapeValuesOnSendCustomized ? EscapeValuesOnReceive : _unescapeValuesOnSend;
            set {
                _unescapeValuesOnSendCustomized = true;
                _unescapeValuesOnSend = value;
            }
        }

        private bool _unescapeValuesOnSend;
        private bool _unescapeValuesOnSendCustomized;

        private static readonly char[] ReservedChars = { ' ', '\t', '\r', '\n', '=', ';', ',' };

        private static BinaryFormatter Bf => _binaryFormatter ?? (_binaryFormatter = new BinaryFormatter());
        private static BinaryFormatter _binaryFormatter;


        public CookieStorage(bool isLocked = false, CookieContainer container = null, bool ignoreInvalidCookie = false)
        {
            IsLocked = isLocked;
            Container = container ?? new CookieContainer();
            IgnoreInvalidCookie = ignoreInvalidCookie;
        }

        /// <summary>
        /// Добавляет Cookie в хранилище <see cref="CookieContainer"/>.
        /// </summary>
        /// <param name="cookie">Кука</param>
        public void Add(Cookie cookie)
        {
            Container.Add(cookie);
        }

        /// <summary>
        /// Добавляет коллекцию Cookies в хранилище <see cref="CookieContainer"/>.
        /// </summary>
        /// <param name="cookies">Коллекция Cookie</param>
        public void Add(CookieCollection cookies)
        {
            Container.Add(cookies);
        }

        /// <summary>
        /// Добавляет или обновляет существующую Cookie в хранилище <see cref="CookieContainer"/>.
        /// </summary>
        /// <param name="cookie">Кука</param>
        // ReSharper disable once UnusedMember.Global
        public void Set(Cookie cookie)
        {
            cookie.Name = cookie.Name.Trim();
            cookie.Value = cookie.Value.Trim();

            if (ExpireBeforeSet)
                ExpireIfExists(cookie);

            Add(cookie);
        }

        /// <summary>
        /// Добавляет или обновляет существующие Cookies из коллекции в хранилище <see cref="CookieContainer"/>.
        /// </summary>
        /// <param name="cookies">Коллекция Cookie</param>
        public void Set(CookieCollection cookies)
        {
            if (ExpireBeforeSet)
            {
                foreach (Cookie cookie in cookies)
                    ExpireIfExists(cookie);
            }

            Add(cookies);
        }

        /// <inheritdoc cref="Set(System.Net.CookieCollection)"/>
        /// <param name="name">Имя куки</param>
        /// <param name="value">Значение куки</param>
        /// <param name="domain">Домен (без протокола)</param>
        /// <param name="path">Путь</param>
        // ReSharper disable once UnusedMember.Global
        public void Set(string name, string value, string domain, string path = "/")
        {
            var cookie = new Cookie(name, value, path, domain);
            Set(cookie);
        }

        /// <inheritdoc cref="Set(System.Net.CookieCollection)"/>
        /// <param name="requestAddress">Адрес запроса</param>
        /// <param name="rawCookie">Сырой формат записи в виде строки</param>
        public void Set(Uri requestAddress, string rawCookie)
        {
            // Отделяем Cross-domain cookie - если не делать, будет исключение.
            // Разделяем все key=value
            var arguments = rawCookie.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            if (arguments.Length == 0)
                return;

            // Получаем ключ и значение самой Cookie
            var keyValue = arguments[0].Split(new[] {'='}, 2);
            if (keyValue.Length <= 1)
                return;
            
            keyValue[0] = keyValue[0].Trim();
            keyValue[1] = keyValue[1].Trim();

            if (IgnoreInvalidCookie && (string.IsNullOrEmpty(keyValue[0]) || keyValue[0][0] == '$' || keyValue[0].IndexOfAny(ReservedChars) != -1)) return;

            var cookie = new Cookie(keyValue[0], keyValue.Length < 2 ? string.Empty 
                : EscapeValuesOnReceive ? Uri.EscapeDataString(keyValue[1]) : keyValue[1]
            );

            bool hasDomainKey = false;

            // Обрабатываем дополнительные ключи Cookie
            for (int i = 1; i < arguments.Length; i++)
            {
                var cookieArgsKeyValues = arguments[i].Split(new[] {'='}, 2);

                // Обрабатываем ключи регистронезависимо
                string key = cookieArgsKeyValues[0].Trim().ToLower();
                string value = cookieArgsKeyValues.Length < 2 ? null : cookieArgsKeyValues[1].Trim();

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (key)
                {
                    case "expires":
                        if (!DateTime.TryParse(value, out var expires) || expires.Year >= 9999)
                            expires = new DateTime(9998, 12, 31, 23, 59, 59, DateTimeKind.Local);

                        cookie.Expires = expires;
                        break;

                    case "path":
                        cookie.Path = value;
                        break;
                    case "domain":
                        string domain = CookieFilters.FilterDomain(value);
                        if (domain == null)
                            continue;

                        hasDomainKey = true;
                        cookie.Domain = domain;
                        break;
                    case "secure":
                        cookie.Secure = true;
                        break;
                    case "httponly":
                        cookie.HttpOnly = true;
                        break;
                }
            }

            if (!hasDomainKey)
            {
                if (string.IsNullOrEmpty(cookie.Path) || cookie.Path.StartsWith("/"))
                    cookie.Domain = requestAddress.Host;
                else if (cookie.Path.Contains("."))
                {
                    string domain = cookie.Path;
                    cookie.Domain = domain;
                    cookie.Path = null;
                }
            }
            
            if (IgnoreSetForExpiredCookies && cookie.Expired)
                return;
            
            Set(cookie);
        }


        /// <inheritdoc cref="Set(System.Net.CookieCollection)"/>
        /// <param name="requestAddress">Адрес запроса</param>
        /// <param name="rawCookie">Сырой формат записи в виде строки</param>
        // ReSharper disable once UnusedMember.Global
        public void Set(string requestAddress, string rawCookie)
        {
            Set(new Uri(requestAddress), rawCookie);
        }

        private void ExpireIfExists(Uri uri, string cookieName)
        {
            var cookies = Container.GetCookies(uri);
            foreach (Cookie storageCookie in cookies)
            {
                if (storageCookie.Name == cookieName)
                    storageCookie.Expired = true;
            }
        }

        private void ExpireIfExists(Cookie cookie)
        {
            if (string.IsNullOrEmpty(cookie.Domain)) 
                return;

            // Fast trim: Domain.Remove is slower and much more slower variation: cookie.Domain.TrimStart('.')
            string domain = cookie.Domain[0] == '.' ? cookie.Domain.Substring(1) : cookie.Domain;
            var uri = new Uri($"{(cookie.Secure ? "https://" : "http://")}{domain}");

            ExpireIfExists(uri, cookie.Name);
        }

        /// <summary>
        /// Очистить <see cref="CookieContainer"/>.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void Clear()
        {
            Container = new CookieContainer();
        }

        /// <summary>
        /// Удалить все <see cref="Cookie"/> связанные с URL адресом.
        /// </summary>
        /// <param name="url">URL адрес ресурса</param>
        public void Remove(string url)
        {
            Remove(new Uri(url));
        }

        /// <inheritdoc cref="Remove(string)"/>
        /// <param name="uri">URI адрес ресурса</param>
        public void Remove(Uri uri)
        {
            var cookies = Container.GetCookies(uri);
            foreach (Cookie cookie in cookies)
                cookie.Expired = true;
        }

        /// <summary>
        /// Удалить <see cref="Cookie"/> по имени для определенного URL.
        /// </summary>
        /// <param name="url">URL ресурса</param>
        /// <param name="name">Имя куки которую нужно удалить</param>
        public void Remove(string url, string name)
        {
            Remove(new Uri(url), name);
        }

        /// <inheritdoc cref="Remove(string, string)"/>
        /// <param name="uri">URL ресурса</param>
        /// <param name="name">Имя куки которую нужно удалить</param>
        public void Remove(Uri uri, string name)
        {
            var cookies = Container.GetCookies(uri);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == name)
                    cookie.Expired = true;
            }
        }

        /// <summary>
        /// Получает Cookies в формате строки-заголовка для HTTP запроса (<see cref="HttpRequestHeader"/>).
        /// </summary>
        /// <param name="uri">URI адрес ресурса</param>
        /// <returns>Вернет строку содержащую все куки для адреса.</returns>
        public string GetCookieHeader(Uri uri)
        {
            string header = Container.GetCookieHeader(uri);
            if (!UnescapeValuesOnSend)
                return header;

            // Unescape cookies values
            var sb = new StringBuilder();
            var cookies = header.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string cookie in cookies)
            {
                var kv = cookie.Split(new []{'='}, 2);
                sb.Append(kv[0].Trim());
                sb.Append('=');
                sb.Append(Uri.UnescapeDataString(kv[1].Trim()));
                sb.Append("; ");
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        /// <inheritdoc cref="GetCookieHeader(System.Uri)"/>
        /// <param name="url">URL адрес ресурса</param>
        // ReSharper disable once UnusedMember.Global
        public string GetCookieHeader(string url)
        {
            return GetCookieHeader(new Uri(url));
        }

        /// <summary>
        /// Получает коллекцию всех <see cref="Cookie"/> связанных с адресом ресурса.
        /// </summary>
        /// <param name="uri">URI адрес ресурса</param>
        /// <returns>Вернет коллекцию <see cref="Cookie"/> связанных с адресом ресурса</returns>
        public CookieCollection GetCookies(Uri uri)
        {
            return Container.GetCookies(uri);
        }

        /// <inheritdoc cref="GetCookies(System.Uri)"/>
        /// <param name="url">URL адрес ресурса</param>
        public CookieCollection GetCookies(string url)
        {
            return GetCookies(new Uri(url));
        }

        /// <summary>
        /// Проверяет существование <see cref="Cookie"/> в <see cref="CookieContainer"/> по адресу ресурса и имени ключа куки.
        /// </summary>
        /// <param name="uri">URI адрес ресурса</param>
        /// <param name="cookieName">Имя-ключ куки</param>
        /// <returns>Вернет <see langword="true"/> если ключ найден по запросу.</returns>
        public bool Contains(Uri uri, string cookieName)
        {
            if (Container.Count <= 0)
                return false;

            var cookies = Container.GetCookies(uri);
            return cookies[cookieName] != null;
        }

        /// <inheritdoc cref="Contains(System.Uri, string)"/>
        public bool Contains(string url, string cookieName)
        {
            return Contains(new Uri(url), cookieName);
        }

        
        #region Load / Save: File
        
        /// <summary>
        /// Сохраняет куки в файл.
        /// <remarks>Рекомендуется расширение .jar.</remarks>
        /// </summary>
        /// <param name="filePath">Пусть для сохранения файла</param>
        /// <param name="overwrite">Перезаписать файл если он уже существует</param>
        // ReSharper disable once UnusedMember.Global
        public void SaveToFile(string filePath, bool overwrite = true)
        {
            if (!overwrite && File.Exists(filePath))
                throw new ArgumentException(string.Format(Resources.CookieStorage_SaveToFile_FileAlreadyExists, filePath), nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                Bf.Serialize(fs, this);
        }

        /// <summary>
        /// Загружает <see cref="CookieStorage"/> из файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу с куками</param>
        /// <returns>Вернет <see cref="CookieStorage"/>, который задается в свойстве <see cref="HttpRequest"/> Cookies.</returns>
        // ReSharper disable once UnusedMember.Global
        public static CookieStorage LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл с куками '${filePath}' не найден", nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open))
                return (CookieStorage)Bf.Deserialize(fs);
        }
        
        #endregion


        #region Save / Load: Bytes
        
        /// <summary>
        /// Сохраняет куки в массив байт.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public byte[] ToBytes()
        {
            byte[] r;

            using (var ms = new MemoryStream())
            {
                Bf.Serialize(ms, this);
                r = ms.ToArray();
            }

            return r;
        }
        
        /// <summary>
        /// Загружает <see cref="CookieStorage"/> из массива байт.
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        /// <returns>Вернет <see cref="CookieStorage"/>, который задается в свойстве <see cref="HttpRequest"/> Cookies.</returns>
        // ReSharper disable once UnusedMember.Global
        public static CookieStorage FromBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return (CookieStorage)Bf.Deserialize(ms);
        }
        
        #endregion
    }
}
