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
        public AnticaptchaSolver(string apiKey) : base("https://api.anti-captcha.com", apiKey) { }

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
    }
}
