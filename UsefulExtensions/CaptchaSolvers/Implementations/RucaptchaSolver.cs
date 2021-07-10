using UsefulExtensions.CaptchaSolvers.Exceptions;
using Leaf.xNet;
using Newtonsoft.Json;
using System.Threading;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICaptchaSolver"/> для сервиса RuCaptcha (https://rucaptcha.com/)
    /// </summary>
    public class RucaptchaSolver : ICaptchaSolver
    {
        private const string CAPTCHA_NOT_READY = "CAPCHA_NOT_READY";

        public event OnLogMessageHandler OnLogMessage;

        public ProxyClient Proxy { get; set; }

        public string Key { get; }

        public RucaptchaSolver(string key)
        {
            Key = key;
        }

        public string SolveArkoseCaptcha(string publicKey, string surl, string pageUrl)
        {
            string inUrl = $"http://rucaptcha.com/in.php?" +
                           $"key={Key}" +
                           $"&method=funcaptcha" +
                           $"&publickey={publicKey}" +
                           $"&surl={surl}" +
                           $"&pageurl={pageUrl}" +
                           $"&json=1";

            return GetAnswer(inUrl);
        }

        private string GetAnswer(string inUrl)
        {
            HttpRequest request = new HttpRequest();
            if (Proxy != null)
                request.Proxy = Proxy;

            string inResponseString = request.Get(inUrl).ToString();
            RucaptchaResponse inResponse = JsonConvert.DeserializeObject<RucaptchaResponse>(inResponseString);

            if (inResponse.Status != 1)
                throw new InvalidRequestException(inResponse.Request);

            OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha sended | ID = {inResponse.Request}"));

            RucaptchaResponse solveResponse = new RucaptchaResponse() { Status = 0, Request = CAPTCHA_NOT_READY };

            while (solveResponse.Status == 0)
            {
                Thread.Sleep(5000);
                string solveResponseString = request.Get($"http://rucaptcha.com/res.php?key={Key}&action=get&json=1&id={inResponse.Request}").ToString();
                solveResponse = JsonConvert.DeserializeObject<RucaptchaResponse>(solveResponseString);
                if (solveResponse.Status == 0 && solveResponse.Request != CAPTCHA_NOT_READY)
                    throw new InvalidRequestException(solveResponse.Request);
                OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha result: {solveResponse.Request}"));
            }

            return solveResponse.Request;
        }

        public string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible)
        {
            string inUrl = $"http://rucaptcha.com/in.php?" +
                           $"key={Key}" +
                           $"&method=userrecaptcha" +
                           $"&googlekey={siteKey}" +
                           $"&pageurl={pageUrl}" +
                           $"&invisible={(invisible ? "1" : "0")}" +
                           $"&json=1";

            return GetAnswer(inUrl);
        }

        public string SolveHCaptcha(string siteKey, string pageUrl)
        {
            string inUrl = $"http://rucaptcha.com/in.php?" +
                           $"key={Key}" +
                           $"&method=hcaptcha" +
                           $"&sitekey={siteKey}" +
                           $"&pageurl={pageUrl}" +
                           $"&json=1";
            
            return GetAnswer(inUrl);
        }
    }

    class RucaptchaResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }
    }
}
