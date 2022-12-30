using Leaf.xNet;
using System;
using UsefulExtensions.CaptchaSolvers.Models;

namespace UsefulExtensions.CaptchaSolvers
{
    /// <summary>
    /// Интерфейс взаимодействия с сервисом решения капчи. Стандартные реализации: <see cref="UsefulExtensions.CaptchaSolvers.Implementations.RucaptchaSolver"/>, <see cref="UsefulExtensions.CaptchaSolvers.Implementations.AnticaptchaSolver"/>
    /// </summary>
    public interface ICaptchaSolver
    {
        /// <summary>
        /// Решение reCAPTCHA V2, также известной как "Я не робот" reCAPTCHA
        /// </summary>
        /// <param name="siteKey">Значение параметра k или data-sitekey, которое вы нашли в коде страницы</param>
        /// <param name="pageUrl">Полный URL страницы, на которой вы решаете reCAPTCHA V2</param>
        /// <param name="invisible">true — говорит нам, что на сайте невидимая reCAPTCHA. false — обычная reCAPTCHA.</param>
        /// <returns>Токен g-recaptcha-response</returns>
        /// <remarks>
        /// Посмотрите исходный код элемента на странице, где вы встретили reCAPTCHA.
        /// <para/> Найдите ссылку, которая начинается с www.google.com/recaptcha/api2/anchor или найдите параметр data-sitekey.
        /// <para/> В аргументы передавайте значение параметра k из ссылки или значение data-sitekey.
        /// </remarks>
        string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteKey"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        string SolveRecaptchaV2Enterprise(string siteKey, string pageUrl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteKey"></param>
        /// <param name="pageUrl"></param>
        /// <param name="action"></param>
        /// <param name="domain"></param>
        /// <param name="minScore"></param>
        /// <returns></returns>
        string SolveRecaptchaV3(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteKey"></param>
        /// <param name="pageUrl"></param>
        /// <param name="action"></param>
        /// <param name="domain"></param>
        /// <param name="minScore"></param>
        /// <returns></returns>
        string SolveRecaptchaV3Enterprise(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3);

        /// <summary>
        /// Решение Arkose Labs FunCaptcha с помощью токена
        /// </summary>
        /// <param name="publicKey">Значение параметра pk или data-pkey которое вы нашли в коде страницы</param>
        /// <param name="surl">Значение параметра surl которое вы нашли в коде страницы</param>
        /// <param name="pageUrl">Полный URL страницы, на которой вы решаете FunCaptcha</param>
        /// <returns>Токен решенной капчи</returns>
        /// <remarks>
        /// Вам нужно найти публичный ключ FunCaptcha и сервисный URL (surl). 
        /// <para/> Публичный ключ можно найти в значении параметра data-pkey у div с FunCaptcha или же найти элемент с именем fc-token, а из его значения вырезать ключ, который указан после pk.
        /// <para/> Сервисный URL можно найти в том же элементе с именем fc-token - это значение после surl. Этот параметр не обязателен и если вы его не указали - мы используем значение по умолчанию, которое работает в большинстве случаев. Но все же мы рекомендуем его указывать на случай, если на сайте используется нестандартный вариант.
        /// </remarks>
        string SolveArkoseCaptcha(string publicKey, string surl, string pageUrl);

        /// <summary>
        /// Решение hCaptcha, относительно новый вид капчи, который очень похож на reCAPTCHA
        /// </summary>
        /// <param name="siteKey">Значение параметра data-sitekey, которое вы нашли в коде страницы</param>
        /// <param name="pageUrl">Полный URL страницы, на которой вы решаете hCaptcha</param>
        /// <param name="invisible">Используйте значение <see langword="true"/> для невидимой версии hcaptcha (в настоящее время встречается крайне редко)</param>
        /// <param name="additionalData">Дополнительные данные для решения капчи - используется в очень редких случаях и только в сочетании с invisible=1</param>
        /// <returns>Токен решенной капчи</returns>
        string SolveHCaptcha(string siteKey, string pageUrl, bool invisible = false, string additionalData = null);


        /// <summary>
        /// Решение Geetest - такого вида капчи, где требуется передвинуть кусок пазла или выбрать несколько объектов в нужном порядке.
        /// </summary>
        /// <param name="gt">Значение параметра gt, найденное на сайте (Публичный ключ домена, редко обновляется)</param>
        /// <param name="challenge">Значение параметра challenge, найденное на сайте (Меняющийся ключ. Убедитесь что получаете каждый раз новый ключ для каждой капчи, иначе вы будете платить за каждую капчу с ошибкой)</param>
        /// <param name="pageUrl"></param>
        /// <param name="apiServer">Значение параметра api_server, найденное на сайте (Опциональный поддомен API. Может потребоваться для некоторых имплементаций)</param>
        /// <returns></returns>
        GeeTestV3CaptchaResult SolveGeeTestV3Captcha(string gt, string challenge, string pageUrl, string apiServer = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="captchaId">Значение captcha_id - обычно его можно найти внутри тега script, который подключает javascript код Geetest v4 на странице (на антикапче этот параметр приравнивается к gt, по сути менятся не должен)</param>
        /// <param name="pageUrl">Адрес целевой страницы. Может находиться в любом месте сайта, в том числе в закрытом для подписчиков разделе. Наши работники не посещают сайт, а вместо этого эмулируют посещение страницы.</param>
        /// <param name="apiServer">Значение параметра api_server, найденное на сайте (Опциональный поддомен API. Может потребоваться для некоторых имплементаций)</param>
        /// <param name="geetestGetLib"></param>
        /// <param name="initParametets"></param>
        /// <returns></returns>
        GeeTestV4CaptchaResult SolveGeeTestV4Captcha(string captchaId, string pageUrl, string apiServer = null, string geetestGetLib = null, object initParametets = null);

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
