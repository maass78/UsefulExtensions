using UsefulExtensions.CaptchaSolvers.Exceptions;
using Leaf.xNet;
using Newtonsoft.Json;
using System.Threading;
using System;
using UsefulExtensions.CaptchaSolvers.Models;
using System.Collections.Generic;
using UsefulExtensions.CaptchaSolvers.Implementations.Anticaptcha;
using System.Linq;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICaptchaSolver"/> для сервиса AntiCaptcha (https://anti-captcha.com/)
    /// </summary>
    public class AnticaptchaSolver : ICaptchaSolver
    {
        private const string CAPTCHA_READY = "ready";

        public string Key { get; }

        public ProxyClient Proxy { get; set; }

        public event OnLogMessageHandler OnLogMessage;

        private TimeSpan _delay = TimeSpan.FromSeconds(5);
        public TimeSpan SolveDelay { get => _delay; set => _delay = value; }
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public AnticaptchaSolver(string key)
        {
            Key = key;
        }

        public string SolveArkoseCaptcha(string publicKey, string surl, string pageUrl)
        {
            var task = new ArkoseCaptchaTask()
            {
                WebsitePublicKey = publicKey,
                WebsiteURL = pageUrl,
                FuncaptchaApiJSSubdomain = surl
            };

            return GetTaskResult<ArkoseCaptchaSolution, ArkoseCaptchaTask>(task).Token;
        }

        private T GetTaskResult<T, Y>(Y task) where Y : AnticaptchaTask
        {
            using (HttpRequest request = new HttpRequest())
            {
                request.ReadWriteTimeout = request.KeepAliveTimeout 
                    = request.ConnectTimeout = (int)RequestTimeout.TotalMilliseconds;

                if (Proxy != null)
                    request.Proxy = Proxy;

                request.AddHeader("Content-Type", "application/json");

                var createTaskRequest = new AnticaptchaСreateTaskRequest<Y>()
                {
                    ClientKey = Key,
                    Task = task
                };

                var inResponse = JsonConvert.DeserializeObject<AnticaptchaCreateTaskResult>(
                    request.Post("http://api.anti-captcha.com/createTask",
                    new StringContent(JsonConvert.SerializeObject(createTaskRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } ))).ToString());

                if (inResponse.ErrorId != 0)
                    throw new CaptchaSolvingException(inResponse.ErrorId, this, $"Captcha error: {inResponse.ErrorCode} ({inResponse.ErrorDescription}) ID: {inResponse.ErrorId}");

                OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha sended | ID = {inResponse.TaskId}"));

                var taskResult = new AnticaptchaGetTaskResult<T>();
                while (taskResult.Status != CAPTCHA_READY)
                {
                    Thread.Sleep(_delay);

                    request.AddHeader("Content-Type", "application/json");

                    string getSolution = JsonConvert.SerializeObject(new AnticaptchaGetTaskRequest() { ClientKey = Key, TaskId = inResponse.TaskId });

                    taskResult = JsonConvert.DeserializeObject<AnticaptchaGetTaskResult<T>>(request.Post("https://api.anti-captcha.com/getTaskResult", new StringContent(getSolution)).ToString());
                    
                    if (taskResult.ErrorId != 0)
                        throw new CaptchaSolvingException(taskResult.ErrorId, this, $"Captcha error: {taskResult.ErrorCode} ({taskResult.ErrorDescription}) ID: {taskResult.ErrorId}");

                    OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha status: {taskResult.Status}"));
                }

                return taskResult.Solution;
            }
        }

        public string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible)
        {
            var task = new RecaptchaV2Task()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                IsInvisible = invisible
            };

            return GetTaskResult<RecaptchaV2, RecaptchaV2Task>(task).GRecaptchaResponse;
        }

        public string SolveRecaptchaV2Enterprise(string siteKey, string pageUrl)
        {
            var task = new RecaptchaV2EnterpriseTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                EnterprisePayload = null
            };

            return GetTaskResult<RecaptchaV2, RecaptchaV2EnterpriseTask>(task).GRecaptchaResponse;
        }

        public string SolveRecaptchaV3(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3)
        {
            var task = new RecaptchaV3Task()
            {
                IsEnterprise = false,
                MinScore = minScore,
                PageAction = action,
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                ApiDomain = domain
            };

            return GetTaskResult<RecaptchaV2, RecaptchaV3Task>(task).GRecaptchaResponse;
        }

        public string SolveRecaptchaV3Enterprise(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3)
        {
            var task = new RecaptchaV3Task()
            {
                IsEnterprise = true,
                PageAction = action,
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                MinScore = minScore,
                ApiDomain = domain
            };

            return GetTaskResult<RecaptchaV2, RecaptchaV3Task>(task).GRecaptchaResponse;
        }

        public string SolveHCaptcha(string siteKey, string pageUrl, bool invisible = false, string additionalData = null)
        {
            var task = new HCaptchaTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl
            };

            return GetTaskResult<RecaptchaV2, HCaptchaTask>(task).GRecaptchaResponse;
        }

        public GeeTestV3CaptchaResult SolveGeeTestV3Captcha(string gt, string challenge, string pageUrl, string apiServer = null)
        {
            var task = new GeeTestTask()
            {
                Gt = gt,
                Challenge = challenge,
                Version = 3,
                WebsiteURL = pageUrl,
                GeetestApiServerSubdomain = apiServer,
            };

            var result = GetTaskResult<GeeTestV3Solution, GeeTestTask>(task);

            return new GeeTestV3CaptchaResult()
            {
                Challenge = result.Challenge,
                Validate = result.Validate,
                Seccode = result.Seccode
            };
        }

        public GeeTestV4CaptchaResult SolveGeeTestV4Captcha(string captchaId, string pageUrl, string apiServer = null, string geetestGetLib = null, object initParametets = null)
        {
            var task = new GeeTestTask()
            {
                Gt = captchaId,
                Version = 4,
                WebsiteURL = pageUrl,
                GeetestApiServerSubdomain = apiServer,
                GeetestGetLib = geetestGetLib,
                InitParameters = initParametets
            };

            var result = GetTaskResult<GeeTestV4Solution, GeeTestTask>(task);

            return new GeeTestV4CaptchaResult()
            {
                CaptchaId = result.CaptchaId,
                CaptchaOutput = result.CaptchaOutput,
                GenTime = result.GenTime,
                LotNumber = result.LotNumber,
                PassToken = result.PassToken
            };
        }

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
        /// <returns>Данные браузера работника (куки, локальное хранилище и т. д.)</returns>
        public CustomSolution SolveCustomCaptcha<T>(string websiteUrl, string templateName, T variables, 
            string proxyAddress = null, int? proxyPort = null, string proxyLogin = null, string proxyPassword = null, List<string> domainsOfInterest = null)
        {
            var task = new CustomTask<T>()
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

            return GetTaskResult<CustomSolution, CustomTask<T>>(task);
        }

    }

   

    class AnticaptchaСreateTaskRequest<T>
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("task")]
        public T Task { get; set; }
    }
    class AnticaptchaCreateTaskResult
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }
    }

    class AnticaptchaGetTaskRequest
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }
    }
    class AnticaptchaGetTaskResult<T>
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("solution")]
        public T Solution { get; set; }

        [JsonProperty("cost")]
        public string Cost { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("createTime")]
        public int CreateTime { get; set; }

        [JsonProperty("endTime")]
        public int EndTime { get; set; }

        [JsonProperty("solveCount")]
        public string SolveCount { get; set; }
    }

   
}
