using Leaf.xNet;
using System;
using UsefulExtensions.CaptchaSolvers.Models;

namespace UsefulExtensions.CaptchaSolvers
{
    /// <summary>
    /// Интерфейс взаимодействия с сервисом решения капчи. Стандартные реализации: <see cref="UsefulExtensions.CaptchaSolvers.Implementations.RucaptchaSolver"/>, 
    /// <see cref="UsefulExtensions.CaptchaSolvers.Implementations.AnticaptchaSolver"/>
    /// </summary>
    public interface ICaptchaSolver
    {
        /// <summary>
        /// Решает текстовую капчу
        /// </summary>
        /// <param name="base64image">Изображение, закодированное в base64 (без всяких префиксов по типу "data:image/png")</param>
        /// <param name="phrase">Если <see langword="true"/>, то должны быть пробелы в капчи</param>
        /// <param name="regsense">Если <see langword="true"/>, то капча должна быть чувствительна к регистру</param>
        /// <param name="math">Если <see langword="true"/>, то капча должна содержать математическое выражение</param>
        /// <param name="numeric">Что должно встречаться в капче. Или без разницы, или только цифры, или только буквы</param>
        /// <param name="minLength">Минимальная длина. 0 - без ограничений</param>
        /// <param name="maxLength">Максимальная длина. 0 - без ограничений</param>
        /// <param name="comment">Текстовая инструкция для работника</param>
        /// <param name="b64imgInstruction">Дополнительное изображение с инструкцией, которое будет показано работникам. Изображение должно быть закодировано в формат Base64.</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Текст решенной капчи</returns>
        string SolveTextCaptcha(string base64image, bool phrase = false, bool regsense = false, bool math = false, 
            Numeric numeric = Numeric.None, int minLength = 0, int maxLength = 0, string comment = null, string b64imgInstruction = null);

        /// <summary>
        /// Решение recaptcha v2
        /// </summary>
        /// <param name="siteKey">reCAPTCHA sitekey. Свойство data-sitekey для reCAPTCHA, которое можно найти внутри элемента div или внутри параметра k запросов к reCAPTHCHA API. Вы также можете использовать script, чтобы найти значение sitekey.</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="invisible"><see langword="true"/> — на сайте невидимая рекапча. <see langword="false"/> — обычная рекапча.</param>
        /// <param name="recaptchaDataSValue">Значение параметра data-s. Может потребоваться для обхода капчи в сервисах Google.</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Токен g-recaptcha-response</returns>
        string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible, string recaptchaDataSValue = null);

        /// <summary>
        /// Решение recaptcha v2 enterprise. Точно такая же recaptcha v2, только использующая Enterprise API
        /// </summary>
        /// <param name="siteKey">Значение параметра k или data-sitekey, которое вы нашли в коде страницы</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="invisible"><see langword="true"/> — на сайте невидимая рекапча. <see langword="false"/> — обычная рекапча.</param>
        /// <param name="enterprisePayload">Дополнительные параметры, передаваемые вызову grecaptcha.enterprise.render. Например, может существовать объект, содержащий значение s.</param>
        /// <param name="apiDomain">Домен, с которого загружается капча: recaptcha.net или google.com. По умолчанию используется google.com.</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Токен g-recaptcha-response</returns>
        string SolveRecaptchaV2Enterprise(string siteKey, string pageUrl, object enterprisePayload = null, string apiDomain = null);

        /// <summary>
        /// Решение recaptcha v3
        /// </summary>
        /// <param name="siteKey">reCAPTCHA sitekey. Свойство data-sitekey для reCAPTCHA, которое можно найти внутри элемента div или внутри параметра k запросов к reCAPTHCHA API. Вы также можете использовать script, чтобы найти значение sitekey.</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="pageAction">Значение параметра action. Значение устанавливается владельцем веб-сайта внутри свойства data-action элемента reCAPTCHA div или передается внутри объекта options при вызове метода execute, например grecaptcha.execute('websiteKey'{ action: 'MyAction' })</param>
        /// <param name="domain">Используйте этот параметр чтобы прислать доменное имя с которого мы должны загружать скрипты рекапчи. Может иметь только одно из этих двух значений: "www.google.com" или "www.recaptcha.net". Не используйте этот параметр, если не понимаете зачем он нужен.</param>
        /// <param name="isEnterprise">Установите этот флаг в "true" если вы хотите решить эту рекапчу как Enterprise. Значение по-умолчанию равно "false" и рекапча будет решена через обычное API. Может быть определено по вызову javascript: grecaptcha.enterprise.execute('site_key', {..})</param>
        /// <param name="minScore">Фильтрует работников с требуемым score. Значение может быть одним из: 0.3 0.7 0.9</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Токен g-recaptcha-response</returns>
        string SolveRecaptchaV3(string siteKey, string pageUrl, double minScore = 0.3, string pageAction = null, bool isEnterprise = false, string domain = null);


        /// <summary>
        /// Решение hCaptcha
        /// </summary>
        /// <param name="siteKey">Значение параметра data-sitekey, которое вы нашли в коде страницы</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="isInvisible">Используйте значение <see langword="true"/> для невидимой версии hcaptcha (в настоящее время встречается крайне редко)</param>
        /// <param name="enterprisePayload">Дополнительные данные для решения капчи - используется в очень редких случаях и только в сочетании с invisible=1. Cодержит дополнительные параметры, такие как: rqdata, sentry, apiEndpoint, endpoint, reportapi, assethost, imghost.</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Обьект данных решенной капчи</returns>
        HCaptchaResult SolveHCaptcha(string siteKey, string pageUrl, bool isInvisible = false, object enterprisePayload = null);


        /// <summary>
        /// Решение Arkose Labs FunCaptcha
        /// </summary>
        /// <param name="publicKey">Публичный ключ ArkoseLabs CAPTCHA. Публичный ключ можно найти в значении параметра data-pkey у div с FunCaptcha или же найти элемент с именем (name) fc-token, а из его значения вырезать ключ, который указан после pk.</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="funcaptchaApiJSSubdomain">Пользовательский поддомен, используемый для загрузки виджета captcha, например: sample-api.arkoselabs.com</param>
        /// <param name="data">Дополнительный объект полезной нагрузки данных преобразован в строку с помощью JSON.stringify. Пример: {\"blob\":\"BLOB_DATA_VALUE\"}</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Токен решенной капчи</returns>
        string SolveArkoseCaptcha(string publicKey, string pageUrl, string funcaptchaApiJSSubdomain = null, string data = null);


        /// <summary>
        /// Решение Geetest v3
        /// </summary>
        /// <param name="gt">Значение параметра gt, найденное на сайте (Публичный ключ домена, редко обновляется)</param>
        /// <param name="challenge">Значение параметра challenge, найденное на сайте (Меняющийся ключ. Убедитесь что получаете каждый раз новый ключ для каждой капчи, иначе вы будете платить за каждую капчу с ошибкой)</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="apiServer">Значение параметра api_server, найденное на сайте (Опциональный поддомен API. Может потребоваться для некоторых имплементаций)</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Обьект данных решенной капчи</returns>
        GeeTestV3CaptchaResult SolveGeeTestV3Captcha(string gt, string challenge, string pageUrl, string apiServer = null);

        /// <summary>
        /// Решение Geetest v4
        /// </summary>
        /// <param name="gt">Значение captcha_id - обычно его можно найти внутри тега script, который подключает javascript код Geetest v4 на странице (на антикапче этот параметр приравнивается к gt, по сути менятся не должен)</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <param name="apiServer">Значение параметра api_server, найденное на сайте (Опциональный поддомен API. Может потребоваться для некоторых имплементаций)</param>
        /// <param name="initParametets">Обязательно для GeeTest V4. Параметр, который передается в вызове функции initGeetest4, обязательно содержит значение captcha_id. Пример использования: {"captcha_id" : "e392e1d7fd421dc63325744d5a2b9c73"}</param>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Обьект данных решенной капчи</returns>
        GeeTestV4CaptchaResult SolveGeeTestV4Captcha(string gt, string pageUrl, object initParametets, string apiServer = null);

        /// <summary>
        /// Прокси
        /// </summary>
        ProxyClient Proxy { get; set; }

        /// <summary>
        /// Вызывается, когда капча отправляется/решается
        /// </summary>
        event OnLogMessageHandler OnLogMessage;

        /// <summary>
        /// Api Key (Сервисный ключ)
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Задержка между проверками на решение (по умолчанию равна 5 сек)
        /// </summary>
        TimeSpan SolveDelay { get; set; }

        /// <summary>
        /// Таймаут для запросов к сервису решения капчи (по умолчанию - 60 сек.)
        /// </summary>
        TimeSpan RequestTimeout { get; set; }
    }

    public delegate void OnLogMessageHandler(object sender, OnLogMessageEventArgs eventArgs);

    public class OnLogMessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public OnLogMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
