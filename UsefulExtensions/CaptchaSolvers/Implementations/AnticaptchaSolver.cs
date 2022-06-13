using UsefulExtensions.CaptchaSolvers.Exceptions;
using Leaf.xNet;
using Newtonsoft.Json;
using System.Threading;
using System;
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
            using (HttpRequest request = new HttpRequest())
            {
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
                    new StringContent(JsonConvert.SerializeObject(createTaskRequest))).ToString());

                if (inResponse.ErrorId != 0)
                    throw new InvalidRequestException("Captcha error: " + inResponse.ErrorId.ToString());

                OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha sended | ID = {inResponse.TaskId}"));

                var taskResult = new AnticaptchaGetTaskResult<T>();
                while (taskResult.Status != CAPTCHA_READY)
                {
                    Thread.Sleep(_delay);
                    
                    request.AddHeader("Content-Type", "application/json");
                    
                    string getSolution = JsonConvert.SerializeObject(new AnticaptchaGetTaskRequest() { ClientKey = Key, TaskId = inResponse.TaskId });
                    
                    taskResult = JsonConvert.DeserializeObject<AnticaptchaGetTaskResult<T>>(request.Post("https://api.anti-captcha.com/getTaskResult", new StringContent(getSolution)).ToString());

                    OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha status: {taskResult.Status})"));
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

    class RecaptchaV2Solution
    {
        [JsonProperty("gRecaptchaResponse")]
        public string GRecaptchaResponse { get; set; }
    }
    class ArkoseCaptchaSolution
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    class GeeTestV3Solution
    {
        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("validate")]
        public string Validate { get; set; }

        [JsonProperty("seccode")]
        public string Seccode { get; set; }
    }

    class GeeTestV4Solution 
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

    class AnticaptchaTask
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
        public RecaptchaV2Task() : base("RecaptchaV2TaskProxyless") {  }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("isInvisible")]
        public bool IsInvisible { get; set; }
    }
    class ArkoseCaptchaTask : AnticaptchaTask
    {
        public ArkoseCaptchaTask() : base("FunCaptchaTaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("funcaptchaApiJSSubdomain")]
        public string FuncaptchaApiJSSubdomain { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("websitePublicKey")]
        public string WebsitePublicKey { get; set; }
    }
    class HCaptchaTask : AnticaptchaTask
    {
        public HCaptchaTask() : base("HCaptchaTaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }
    }
    class GeeTestTask : AnticaptchaTask
    {
        public GeeTestTask() : base("GeeTestTaskProxyless") { }

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
}
