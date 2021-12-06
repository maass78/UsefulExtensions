using UsefulExtensions.CaptchaSolvers.Exceptions;
using Leaf.xNet;
using Newtonsoft.Json;
using System.Threading;
using System;

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
            var request = new AnticaptchaСreateTaskRequest() { ClientKey = Key, Task = new ArkoseCaptchaTask() { Type = "FunCaptchaTaskProxyless", WebsitePublicKey = publicKey, WebsiteURL = pageUrl, FuncaptchaApiJSSubdomain = surl } };
            return GetTaskResult<ArkoseCaptchaSolution>(request).Token;
        }

        private T GetTaskResult<T>(AnticaptchaСreateTaskRequest taskRequest)
        {
            HttpRequest request = new HttpRequest();
            if (Proxy != null)
                request.Proxy = Proxy;

            request.AddHeader("Content-Type", "application/json");
            var inResponse = JsonConvert.DeserializeObject<AnticaptchaCreateTaskResult>(request.Post("http://api.anti-captcha.com/createTask", new StringContent(JsonConvert.SerializeObject(taskRequest))).ToString());

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

        public string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible)
        {
            var request = new AnticaptchaСreateTaskRequest() { ClientKey = Key, Task = new RecaptchaV2Task() { Type = "RecaptchaV2TaskProxyless", WebsiteKey = siteKey, WebsiteURL = pageUrl, IsInvisible = invisible  } };
            return GetTaskResult<RecaptchaV2Solution>(request).GRecaptchaResponse;
        }

        public string SolveHCaptcha(string siteKey, string pageUrl, bool invisible = false, string additionalData = null)
        {
            var request = new AnticaptchaСreateTaskRequest() { ClientKey = Key, Task = new HCaptchaTask() { Type = "HCaptchaTaskProxyless", WebsiteKey = siteKey, WebsiteURL = pageUrl } };
            return GetTaskResult<RecaptchaV2Solution>(request).GRecaptchaResponse;
        }
    }

    class AnticaptchaСreateTaskRequest
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("task")]
        public object Task { get; set; }
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

    class RecaptchaV2Task
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("isInvisible")]
        public bool IsInvisible { get; set; }
    }
    class ArkoseCaptchaTask
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("funcaptchaApiJSSubdomain")]
        public string FuncaptchaApiJSSubdomain { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("websitePublicKey")]
        public string WebsitePublicKey { get; set; }
    }
    class HCaptchaTask
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }
    }
}
