using Newtonsoft.Json;
using System.Collections.Generic;

namespace UsefulExtensions.CaptchaSolvers.Implementations.Models
{
    internal class RecaptchaSolution
    {
        [JsonProperty("gRecaptchaResponse")]
        public string GRecaptchaResponse { get; set; }
    }

    internal class HCaptchaSolution : RecaptchaSolution
    {
        [JsonProperty("respKey")]
        public string RespKey { get; set; }


        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }
    }

    internal class TextSolution
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    internal class ArkoseCaptchaSolution
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

    internal class YandexSmartCaptchaSolution
    {
        [JsonProperty("token")]
        public string Token { get; set; }
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

        [JsonProperty("screenshots")]
        public List<string> Screenshots { get; set; }

        [JsonProperty("HTMLsInBase64")]
        public List<string> HTMLsInBase64 { get; set; }

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
