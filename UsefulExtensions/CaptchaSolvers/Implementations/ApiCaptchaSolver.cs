using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using UsefulExtensions.CaptchaSolvers.Exceptions;
using UsefulExtensions.CaptchaSolvers.Implementations.Models;
using UsefulExtensions.CaptchaSolvers.Models;
using StringContent = System.Net.Http.StringContent;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Базовый класс для api рукапчи, тукапчи, антикапчи и т. д. (с тех пор как рукапча ввела apiv2)
    /// </summary>
    public class ApiCaptchaSolver : ICaptchaSolver
    {
        private const string CAPTCHA_READY = "ready";

        private TimeSpan _delay = TimeSpan.FromSeconds(5);
        private TimeSpan _requestTimeout = TimeSpan.FromMinutes(1);

        public ApiCaptchaSolver(string baseUrl, string apiKey)
        {
            Key = apiKey;
            BaseUrl = baseUrl;
        }

        /// <inheritdoc/>
        public TimeSpan SolveDelay { get => _delay; set => _delay = value; }

        /// <inheritdoc/>
        public TimeSpan RequestTimeout { get => _requestTimeout; set => _requestTimeout = value; }

        /// <inheritdoc/>
        public string Key { get; }

        /// <inheritdoc/>
        public string BaseUrl { get; }

        /// <inheritdoc/>
        public Leaf.xNet.ProxyClient Proxy { get => null; set { } }

        /// <inheritdoc/>
        public IWebProxy WebProxy { get; set; }

        /// <inheritdoc/>
        public event OnLogMessageHandler OnLogMessage;

        /// <inheritdoc/>
        public string SolveTextCaptcha(string base64image, bool phrase = false, bool regsense = false, bool math = false, Numeric numeric = Numeric.None, int minLength = 0, int maxLength = 0, string comment = null, string b64imgInstruction = null)
        {
            var task = new TextTask()
            {
                Body = base64image,
                Phrase = phrase,
                Case = regsense,
                Math = math,
                MinLength = minLength,
                MaxLength = maxLength,
                Numeric = (int)numeric,
            };

            return GetTaskResult<TextSolution, TextTask>(task).Text;
        }

        /// <inheritdoc/>
        public string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible, string recaptchaDataSValue = null)
        {
            var task = new RecaptchaV2Task()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                IsInvisible = invisible,
                RecaptchaDataSValue = recaptchaDataSValue,
            };

            return GetTaskResult<RecaptchaSolution, RecaptchaV2Task>(task).GRecaptchaResponse;
        }

        /// <inheritdoc/>
        public string SolveRecaptchaV2Enterprise(string siteKey, string pageUrl, object enterprisePayload = null, string apiDomain = null)
        {
            var task = new RecaptchaV2EnterpriseTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                EnterprisePayload = enterprisePayload,
                ApiDomain = apiDomain,
            };

            return GetTaskResult<RecaptchaSolution, RecaptchaV2EnterpriseTask>(task).GRecaptchaResponse;
        }

        /// <inheritdoc/>
        public string SolveRecaptchaV3(string siteKey, string pageUrl, double minScore = 0.3, string pageAction = null, bool isEnterprise = false, string domain = null)
        {
            var task = new RecaptchaV3Task()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                MinScore = minScore,
                PageAction = pageAction,
                IsEnterprise = isEnterprise,
                ApiDomain = domain
            };

            return GetTaskResult<RecaptchaSolution, RecaptchaV3Task>(task).GRecaptchaResponse;
        }

        /// <inheritdoc/>
        public HCaptchaResult SolveHCaptcha(string siteKey, string pageUrl, bool isInvisible = false, object enterprisePayload = null)
        {
            var task = new HCaptchaTask()
            {
                WebsiteKey = siteKey,
                WebsiteURL = pageUrl,
                EnterprisePayload = enterprisePayload,
                IsInvisible = isInvisible,
            };

            var result = GetTaskResult<HCaptchaSolution, HCaptchaTask>(task);

            return new HCaptchaResult()
            {
                GRecaptchaResponse = result.GRecaptchaResponse,
                RespKey = result.RespKey,
                UserAgent = result.UserAgent,
            };
        }
        
        /// <inheritdoc/>
        public string SolveArkoseCaptcha(string publicKey, string pageUrl, string funcaptchaApiJSSubdomain = null, string data = null)
        {
            var task = new ArkoseCaptchaTask()
            {
                WebsitePublicKey = publicKey,
                WebsiteURL = pageUrl,
                FuncaptchaApiJSSubdomain = funcaptchaApiJSSubdomain,
                Data = data,
            };

            return GetTaskResult<ArkoseCaptchaSolution, ArkoseCaptchaTask>(task).Token;
        }
        
        /// <inheritdoc/>
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
        
        /// <inheritdoc/>
        public GeeTestV4CaptchaResult SolveGeeTestV4Captcha(string gt, string pageUrl, object initParametets, string apiServer = null)
        {
            var task = new GeeTestTask()
            {
                Gt = gt,
                Version = 4,
                WebsiteURL = pageUrl,
                GeetestApiServerSubdomain = apiServer,
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
        /// Получить результат капчи
        /// </summary>
        /// <typeparam name="T">Тип результата</typeparam>
        /// <typeparam name="Y">Тип задачи</typeparam>
        /// <param name="task">Задача капчи</param>
        /// <exception cref="CaptchaSolvingException">Ошибка при решении капчи (не хватает баланса, капча нерешаема, неверные параметры и т. д.)</exception>
        protected T GetTaskResult<T, Y>(Y task) where Y : CaptchaTask
        {
            using (var handler = new HttpClientHandler())
            {
                using (var request = new HttpClient())
                {
                    request.Timeout = RequestTimeout;

                    if (WebProxy != null)
                        handler.Proxy = WebProxy;

                    var createTaskRequest = new СreateTaskRequest<Y>()
                    {
                        ClientKey = Key,
                        Task = task
                    };

                    var body = new StringContent(JsonConvert.SerializeObject(createTaskRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));

                    body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var inResponse = JsonConvert.DeserializeObject<CreateTaskResponse>(
                        request.PostAsync($"{BaseUrl}/createTask", body).Result.Content.ReadAsStringAsync().Result);

                    if (inResponse.ErrorId != 0)
                        throw new CaptchaSolvingException(inResponse.ErrorId, this, $"Captcha error: {inResponse.ErrorCode} ({inResponse.ErrorDescription}) ID: {inResponse.ErrorId}");

                    OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha sended | ID = {inResponse.TaskId}"));

                    var taskResult = new GetTaskResponse<T>();
                    while (taskResult.Status != CAPTCHA_READY)
                    {
                        Thread.Sleep(_delay);

                        body = new StringContent(JsonConvert.SerializeObject(new GetTaskRequest() { ClientKey = Key, TaskId = inResponse.TaskId }));
                        body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        taskResult = JsonConvert.DeserializeObject<GetTaskResponse<T>>(request.PostAsync($"{BaseUrl}/getTaskResult", body).Result.Content.ReadAsStringAsync().Result);

                        if (taskResult.ErrorId != 0)
                        {
                            if (typeof(T) == typeof(CustomSolution) && taskResult.ErrorId == 57)
                                throw new CustomCaptchaSolvingException(taskResult.ErrorId, this, $"Captcha error: {taskResult.ErrorCode} ({taskResult.ErrorDescription}) ID: {taskResult.ErrorId}")
                                {
                                    ErrorMessage = taskResult.ErrorDescription,
                                    ScreenshotUrl = taskResult.Screenshot
                                };

                            throw new CaptchaSolvingException(taskResult.ErrorId, this, $"Captcha error: {taskResult.ErrorCode} ({taskResult.ErrorDescription}) ID: {taskResult.ErrorId}");
                        }

                        OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha status: {taskResult.Status}"));
                    }

                    return taskResult.Solution;
                }
            }
        }
    }
}
