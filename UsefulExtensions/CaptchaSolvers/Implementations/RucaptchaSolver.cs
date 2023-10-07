using UsefulExtensions.CaptchaSolvers.Implementations.Models;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICaptchaSolver"/> для сервиса RuCaptcha (<see href="https://rucaptcha.com/"/>)
    /// </summary>
    public class RucaptchaSolver : ApiCaptchaSolver
    {
        public RucaptchaSolver(string apiKey) : base("https://api.rucaptcha.com", apiKey) { }

        /// <summary>
        /// Решение Yandex Smart Captcha
        /// </summary>
        /// <param name="siteKey">Значение параметра Yandex SmartCaptcha sitekey. Параметр sitekey можно найти в URL-адресе iframe captcha или в параметрах вызова smartCaptcha.render.</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <returns></returns>
        public string SolveYandexSmartCaptcha(string siteKey, string pageUrl)
        {
            var task = new YandexSmartCaptchaTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
            };

            return GetTaskResult<YandexSmartCaptchaSolution, YandexSmartCaptchaTask>(task).Token;
        }
    }
}
