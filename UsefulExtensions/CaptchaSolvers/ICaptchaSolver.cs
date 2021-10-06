using Leaf.xNet;
using System;

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
        /// <returns>Токен решенной капчи</returns>
        string SolveHCaptcha(string siteKey, string pageUrl);

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
