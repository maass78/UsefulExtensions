using System;
using System.Collections.Generic;
using System.Threading;
using Leaf.xNet;
using Newtonsoft.Json;
using UsefulExtensions.CaptchaSolvers.Exceptions;
using UsefulExtensions.CaptchaSolvers.Models;

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
            using(HttpRequest request = new HttpRequest())
            {
                if(Proxy != null)
                    request.Proxy = Proxy;

                request.AddHeader("Content-Type", "application/json");

                var createTaskRequest = new AnticaptchaСreateTaskRequest<Y>()
                {
                    ClientKey = Key,
                    Task = task
                };

                var inResponse = JsonConvert.DeserializeObject<AnticaptchaCreateTaskResult>(
                    request.Post("http://api.anti-captcha.com/createTask",
                    new StringContent(JsonConvert.SerializeObject(createTaskRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }))).ToString());

                if(inResponse.ErrorId != 0)
                    throw new CaptchaSolvingException(inResponse.ErrorId, this, $"Captcha error: {inResponse.ErrorCode} ({inResponse.ErrorDescription}) ID: {inResponse.ErrorId}");

                OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha sended | ID = {inResponse.TaskId}"));

                var taskResult = new AnticaptchaGetTaskResult<T>();
                while(taskResult.Status != CAPTCHA_READY)
                {
                    Thread.Sleep(_delay);

                    request.AddHeader("Content-Type", "application/json");

                    string getSolution = JsonConvert.SerializeObject(new AnticaptchaGetTaskRequest() { ClientKey = Key, TaskId = inResponse.TaskId });

                    taskResult = JsonConvert.DeserializeObject<AnticaptchaGetTaskResult<T>>(request.Post("https://api.anti-captcha.com/getTaskResult", new StringContent(getSolution)).ToString());

                    if(taskResult.ErrorId != 0)
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

            return GetTaskResult<RecaptchaV2Solution, RecaptchaV2Task>(task).GRecaptchaResponse;
        }

        public string SolveHCaptcha(string siteKey, string pageUrl, bool invisible = false, string additionalData = null)
        {
            var task = new HCaptchaTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl
            };

            return GetTaskResult<RecaptchaV2Solution, HCaptchaTask>(task).GRecaptchaResponse;
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

    internal class AnticaptchaСreateTaskRequest<T>
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("task")]
        public T Task { get; set; }
    }

    internal class AnticaptchaCreateTaskResult
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

    internal class AnticaptchaGetTaskRequest
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }
    }

    internal class AnticaptchaGetTaskResult<T>
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

    internal class RecaptchaV2Solution
    {
        [JsonProperty("gRecaptchaResponse")]
        public string GRecaptchaResponse { get; set; }
    }

    class ArkoseCaptchaSolution
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    internal class GeeTestV3Solution
    {
        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("validate")]
        public string Validate { get; set; }

        [JsonProperty("seccode")]
        public string Seccode { get; set; }
    }

    internal class GeeTestV4Solution
    {
        [JsonProperty("captcha_id")]
        public string CaptchaId { get; set; }

        [JsonProperty("lot_number")]
        public string LotNumber { get; set; }

        [JsonProperty("pass_token")]
        public string PassToken { get; set; }

        [JsonProperty("gen_time")]
        public int GenTime { get; set; }

        [JsonProperty("captcha_output")]
        public string CaptchaOutput { get; set; }
    }

    internal class AnticaptchaTask
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public AnticaptchaTask(string type)
        {
            Type = type;
        }
    }

    class RecaptchaV2Task : AnticaptchaTask
    {
        public RecaptchaV2Task() : base("RecaptchaV2TaskProxyless")
        {
        }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("isInvisible")]
        public bool IsInvisible { get; set; }
    }

    internal class ArkoseCaptchaTask : AnticaptchaTask
    {
        public ArkoseCaptchaTask() : base("FunCaptchaTaskProxyless")
        {
        }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("funcaptchaApiJSSubdomain")]
        public string FuncaptchaApiJSSubdomain { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("websitePublicKey")]
        public string WebsitePublicKey { get; set; }
    }

    internal class HCaptchaTask : AnticaptchaTask
    {
        public HCaptchaTask() : base("HCaptchaTaskProxyless")
        {
        }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }
    }

    class GeeTestTask : AnticaptchaTask
    {
        public GeeTestTask() : base("GeeTestTaskProxyless")
        {
        }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("gt")]
        public string Gt { get; set; }

        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("geetestApiServerSubdomain")]
        public string GeetestApiServerSubdomain { get; set; }

        [JsonProperty("geetestGetLib")]
        public string GeetestGetLib { get; set; }

        [JsonProperty("initParameters")]
        public object InitParameters { get; set; }
    }

    internal class CustomTask<T> : AnticaptchaTask
    {
        public CustomTask() : base("AntiGateTask")
        {
        }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("templateName")]
        public string TemplateName { get; set; }

        [JsonProperty("variables")]
        public T Variables { get; set; }

        [JsonProperty("proxyAddress")]
        public string ProxyAddress { get; set; }

        [JsonProperty("proxyPort")]
        public int? ProxyPort { get; set; }

        [JsonProperty("proxyLogin")]
        public string ProxyLogin { get; set; }

        [JsonProperty("proxyPassword")]
        public string ProxyPassword { get; set; }

        [JsonProperty("domainsOfInterest")]
        public List<string> DomainsOfInterest { get; set; }
    }

    public class CustomSolution
    {
        [JsonProperty("cookies")]
        public object Cookies { get; set; }

        [JsonProperty("localStorage")]
        public object LocalStorage { get; set; }

        [JsonProperty("fingerprint")]
        public object Fingerprint { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("domainsOfInterest")]
        public object DomainsOfInterest { get; set; }

        public class DomainOfInterest
        {
            [JsonProperty("cookies")]
            public Dictionary<string, string> Cookies { get; set; }

            [JsonProperty("localStorage")]
            public Dictionary<string, object> LocalStorage { get; set; }

            [JsonProperty("fingerprint")]
            public Dictionary<string, object> Fingerprint { get; set; }
        }
    }
}