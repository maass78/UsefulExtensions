using Org.BouncyCastle.Utilities.Encoders;
using System.Collections.Generic;
using UsefulExtensions.CaptchaSolvers.Implementations.Models;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICaptchaSolver"/> для сервиса RuCaptcha (<see href="https://rucaptcha.com/"/>) API v2
    /// </summary>
    public class RucaptchaSolver : ApiCaptchaSolver
    {
        /// <summary>
        /// Констурктор класса <see cref="RucaptchaSolver"/>
        /// </summary>
        /// <param name="apiKey">API ключ для доступа к сервису</param>
        public RucaptchaSolver(string apiKey) : base("https://api.rucaptcha.com", apiKey) { }
        /// <summary>
        /// Констурктор класса <see cref="RucaptchaSolver"/>
        /// </summary>
        /// <param name="apiKey">API ключ для доступа к сервису</param>
        /// <param name="baseUrl">URL сервера, например <code>https://api.rucaptcha.com</code></param>
        public RucaptchaSolver(string apiKey, string baseUrl) : base(baseUrl, apiKey) { }

        /// <summary>
        /// Решение Yandex Smart Captcha
        /// </summary>
        /// <param name="siteKey">Значение параметра Yandex SmartCaptcha sitekey. Параметр sitekey можно найти в URL-адресе iframe captcha или в параметрах вызова smartCaptcha.render.</param>
        /// <param name="pageUrl">Полный URL-адрес целевой веб-страницы, на которую загружается капча.</param>
        /// <returns>Токен решенной капчи</returns>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        public string SolveYandexSmartCaptcha(string siteKey, string pageUrl)
        {
            var task = new YandexSmartCaptchaTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
            };

            return GetTaskResult<YandexSmartCaptchaSolution, YandexSmartCaptchaTask>(task).Token;
        }

        /// <summary>
        /// Этот метод можно использовать для обхода задач, в которых к изображению применяется сетка и вам нужно щелкнуть по плиткам сетки, например, по изображениям reCAPTCHA или hCaptcha.
        /// </summary>
        /// <remarks>Поддерживаемые форматы изображений: JPEG, PNG, GIF<br/>
        /// Максимальный размер файла: 600 kB<br/>
        /// Максимальный размер изображения: 1000px с любой стороны</remarks>
        /// <param name="b64body">Изображение закодировано в формат Base64. Также поддерживается формат Data-URI (содержащий префикс data:content/type)</param>
        /// <param name="comment">Работникам будет показан комментарий, который поможет им правильно разгадать капчу</param>
        /// <param name="rows">Количество строк в сетке</param>
        /// <param name="columns">Количество столбцов в сетке</param>
        /// <param name="b64imgInstructions">Изображение с инструкцией, которое будет показано работникам. Изображение должно быть закодировано в формат Base64. Максимальный размер файла: 100 kB</param>
        /// <returns>Массив индексов плиток, где 1 - верхняя левая плитка.</returns>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        public List<int> SolveGridCaptcha(string b64body, string comment, int rows, int columns, string b64imgInstructions = null)
        {
            var task = new GridTask()
            {
                Body = b64body,
                Comment = comment,
                Rows = rows,
                Columns = columns,
                ImgInstructions = b64imgInstructions
            };

            return GetTaskResult<GridSolution, GridTask>(task).Click;
        }

        /// <summary>
        /// Этот метод можно использовать для решения задач, в которых вам нужно щелкнуть по определенным точкам на изображении. В ответе вы получите координаты точек в том порядке, в котором на них кликнул работник.
        /// </summary>
        /// <remarks>Поддерживаемые форматы изображений: JPEG, PNG, GIF<br/>
        /// Максимальный размер файла: 600 kB<br/>
        /// Максимальный размер изображения: 1000px с любой стороны</remarks>
        /// <param name="b64body">Изображение, закодированное в формат Base64. Также поддерживается формат Data-URI (содержащий префикс data:content/type)</param>
        /// <param name="comment">Работникам будет показан комментарий, который поможет им правильно разгадать капчу</param>
        /// <param name="b64imgInstructions">Необязательное изображение с инструкцией, которое будет показано работникам. Изображение должно быть закодировано в формат Base64. Максимальный размер файла: 100 kB</param>
        /// <returns>Список <see cref="Coordinate"/>, на которые кликнул работник</returns>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        public List<Coordinate> SolveClickCaptcha(string b64body, string comment, string b64imgInstructions = null)
        {
            var task = new BoundingBoxTask()
            {
                Body = b64body,
                Comment = comment,
                ImgInstructions = b64imgInstructions,
            };

            return GetTaskResult<ClickSolution, BoundingBoxTask>(task).Coordinates;
        }
    }
}
