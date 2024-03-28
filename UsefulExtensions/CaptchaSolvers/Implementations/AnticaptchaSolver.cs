using System.Collections.Generic;
using UsefulExtensions.CaptchaSolvers.Implementations.Models;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICaptchaSolver"/> для сервиса AntiCaptcha (<see href="https://anti-captcha.com/"/>)
    /// </summary>
    public class AnticaptchaSolver : ApiCaptchaSolver
    {
        /// <summary>
        /// Констурктор класса <see cref="AnticaptchaSolver"/>
        /// </summary>
        /// <param name="apiKey">API ключ для доступа к сервису</param>
        public AnticaptchaSolver(string apiKey) : base("http://api.anti-captcha.com", apiKey) { }
        /// <summary>
        /// Констурктор класса <see cref="AnticaptchaSolver"/>
        /// </summary>
        /// <param name="apiKey">API ключ для доступа к сервису</param>
        /// <param name="baseUrl">URL сервера, например <code>https://api.rucaptcha.com</code></param>
        public AnticaptchaSolver(string apiKey, string baseUrl) : base(baseUrl, apiKey) { }

        /// <summary>
        /// Решает кастомную AntiGate задачу
        /// </summary>
        /// <typeparam name="T">Тип объекта параметров</typeparam>
        /// <param name="websiteUrl">Адрес целевой страницы, куда перейдет работник.</param>
        /// <param name="templateName">Название шаблона сценария из нашей базы данных. Вы можете использовать существующий шаблон или создать свой. Можно поискать существующий шаблон на странице https://anti-captcha.com/ru/apidoc/task-types/AntiGateTask.</param>
        /// <param name="variables">Объект, содержащий переменные шаблона и его значения.</param>
        /// <param name="proxyAddress">Адрес прокси в ipv4/ipv6. Имена хостов или адреса из локальной сети не допускаются. Если не надо, указывайте <see langword="null"/>.</param>
        /// <param name="proxyPort">Порт прокси. Если не надо, указывайте <see langword="null"/>.</param>
        /// <param name="proxyLogin">Логин, если требуется авторизация прокси (basic). Если не надо, указывайте <see langword="null"/>.</param>
        /// <param name="proxyPassword">Пароль прокси. Если не надо, указывайте <see langword="null"/>.</param>
        /// <param name="domainsOfInterest">Список доменных имен, где мы должны собрать cookies и значения localStorage. Его также можно задать статично при редактировании шаблона.</param>
        /// <exception cref="Exceptions.CustomCaptchaSolvingException"/>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        /// <returns>Данные браузера работника (куки, локальное хранилище и т. д.)</returns>
        public CustomSolution SolveCustomCaptcha<T>(string websiteUrl, string templateName, T variables, 
            string proxyAddress = null, int? proxyPort = null, string proxyLogin = null, string proxyPassword = null, List<string> domainsOfInterest = null)
        {
            var task = new CustomAnticaptchaTask<T>()
            {
                WebsiteURL = websiteUrl,
                TemplateName = templateName,
                Variables = variables,
                ProxyAddress = proxyAddress,
                ProxyPort = proxyPort,
                ProxyLogin = proxyLogin,
                ProxyPassword = proxyPassword,
                DomainsOfInterest = domainsOfInterest
            };

            return GetTaskResult<CustomSolution, CustomAnticaptchaTask<T>>(task);
        }

        /// <summary>
        /// Пришлите тело изображения, комментарий на английском языке и получите до 6-ти наборов координат нужных объектов. Вы можете запросить координаты точек, а также координаты прямоугольников, в которых находится объект.
        /// </summary>
        /// <remarks>
        /// Максимальный размер изображения по любой из сторон - 500 пикселей. Изображения превышающие данный лимит будут ужаты до 500 пикселей в интерфейсе работника.
        /// </remarks>
        /// <param name="b64body">Тело капчи закодированное в base64. Убедитесь что присылаете его без знаков переноса строки. Не включайте префиксы 'data:image/png,' или аналоги, только чистый base64!</param>
        /// <param name="comment">Комментарий, только на английском языке. Пример: "Select objects in specified order" или "select all cars".</param>
        /// <param name="mode">Режим задачи, может быть <see cref="ImageToCoordinatesMode.Points"/> или <see cref="ImageToCoordinatesMode.Rectangle"/>. По умолчанию - <see cref="ImageToCoordinatesMode.Points"/>.</param>
        /// <param name="websiteURL">Опциональный параметр, чтобы позже различать источники картинок в статистике трат.</param>
        /// <returns>Массив наборов координат. Для режима <see cref="ImageToCoordinatesMode.Points"/> это набор (x, y). Для <see cref="ImageToCoordinatesMode.Rectangle"/> это (x1, y1, x2, y2), начиная с угла сверху-налево во вниз-направо. Начало координат находится левом верхнем углу.</returns>
        /// <exception cref="Exceptions.CaptchaSolvingException"/>
        public List<List<int>> SolveImageToCoordinatesCaptcha(string b64body, string comment, ImageToCoordinatesMode mode = ImageToCoordinatesMode.Points,
            string websiteURL = null)
        {
            var task = new ImageToCoordinatesTask()
            {
                Body = b64body,
                Comment = comment,
                Mode = mode.ToString().ToLower(),
                WebsiteURL = websiteURL
            };

            return GetTaskResult<ImageToCoordinatesSolution, ImageToCoordinatesTask>(task).Coordinates;
        }
    }
}
