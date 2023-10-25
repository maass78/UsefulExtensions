using Newtonsoft.Json;
using System.Collections.Generic;

namespace UsefulExtensions.CaptchaSolvers.Implementations.Models
{
    public class CaptchaTask
    {
        [JsonProperty("type")]
        public string Type { get; }

        public CaptchaTask(string type)
        {
            Type = type;
        }
    }

    internal class RecaptchaV2Task : CaptchaTask
    {
        public RecaptchaV2Task() : base("RecaptchaV2TaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("isInvisible")]
        public bool IsInvisible { get; set; }

        [JsonProperty("recaptchaDataSValue")]
        public string RecaptchaDataSValue { get; set; }
    }

    internal class RecaptchaV2EnterpriseTask : CaptchaTask
    {
        public RecaptchaV2EnterpriseTask() : base("RecaptchaV2EnterpriseTaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("enterprisePayload")]
        public object EnterprisePayload { get; set; }

        [JsonProperty("apiDomain")]
        public string ApiDomain { get; set; }
    }

    internal class RecaptchaV3Task : CaptchaTask
    {
        public RecaptchaV3Task() : base("RecaptchaV3TaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("minScore")]
        public double MinScore { get; set; }

        [JsonProperty("pageAction")]
        public string PageAction { get; set; }

        [JsonProperty("isEnterprise")]
        public bool IsEnterprise { get; set; }

        [JsonProperty("apiDomain")]
        public string ApiDomain { get; set; }
    }

    internal class HCaptchaTask : CaptchaTask
    {
        public HCaptchaTask() : base("HCaptchaTaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }

        [JsonProperty("isInvisible")]
        public bool IsInvisible { get; set; }

        [JsonProperty("enterprisePayload")]
        public object EnterprisePayload { get; set; }
    }

    internal class ArkoseCaptchaTask : CaptchaTask
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
    
    internal class GeeTestTask : CaptchaTask
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

        [JsonProperty("initParameters")]
        public object InitParameters { get; set; }
    }

    internal class CustomAnticaptchaTask<T> : CaptchaTask
    {
        public CustomAnticaptchaTask() : base("AntiGateTask") { }

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

    internal class TextTask : CaptchaTask
    {
        public TextTask() : base("ImageToTextTask") { }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("phrase")]
        public bool Phrase { get; set; }

        [JsonProperty("case")]
        public bool Case { get; set; }

        [JsonProperty("numeric")]
        public int Numeric { get; set; }

        [JsonProperty("math")]
        public bool Math { get; set; }

        [JsonProperty("minLength")]
        public int MinLength { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }
    }

    internal class GridTask : CaptchaTask
    {
        public GridTask() : base("GridTask") { }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("imgInstructions")]
        public string ImgInstructions { get; set; }

        [JsonProperty("rows")]
        public int Rows { get; set; }

        [JsonProperty("columns")]
        public int Columns { get; set; }
    }

    internal class BoundingBoxTask : CaptchaTask
    {
        public BoundingBoxTask() : base("BoundingBoxTask") { }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("imgInstructions")]
        public string ImgInstructions { get; set; }
    }

    internal class ImageToCoordinatesTask : CaptchaTask
    {
        public ImageToCoordinatesTask() : base("ImageToCoordinatesTask") { }


        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }
    }

    public enum ImageToCoordinatesMode
    {
        Points,
        Rectangle
    }

    internal class YandexSmartCaptchaTask : CaptchaTask
    {
        public YandexSmartCaptchaTask() : base("YandexSmartCaptchaTaskProxyless") { }

        [JsonProperty("websiteURL")]
        public string WebsiteURL { get; set; }

        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }
    }
}
