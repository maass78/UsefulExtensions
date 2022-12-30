﻿using UsefulExtensions.CaptchaSolvers.Exceptions;
using Leaf.xNet;
using Newtonsoft.Json;
using System.Threading;
using System;
using UsefulExtensions.CaptchaSolvers.Models;
using System.Globalization;

namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICaptchaSolver"/> для сервиса RuCaptcha (https://rucaptcha.com/)
    /// </summary>
    public class RucaptchaSolver : ICaptchaSolver
    {
        private const string CAPTCHA_NOT_READY = "CAPCHA_NOT_READY";

        private TimeSpan _delay = TimeSpan.FromSeconds(5);

        public event OnLogMessageHandler OnLogMessage;

        public string Key { get; }
        
        public string Url { get; set; }

        public ProxyClient Proxy { get; set; }

        public TimeSpan SolveDelay { get => _delay; set => _delay = value; }
        public TimeSpan RequestTimeout { get; set; }

        public RucaptchaSolver(string key, string url = "http://rucaptcha.com")
        {
            Url = url;
            Key = key;
        }

        public string SolveArkoseCaptcha(string publicKey, string surl, string pageUrl)
        {
            string inUrl = $"{Url}/in.php?" +
                           $"key={Key}" +
                           $"&method=funcaptcha" +
                           $"&publickey={publicKey}" +
                           $"&surl={surl}" +
                           $"&pageurl={System.Web.HttpUtility.UrlEncode(pageUrl)}" +
                           $"&json=1";

            return GetAnswer(inUrl);
        }

        private string GetAnswer(string inUrl)
        {
            using (HttpRequest request = new HttpRequest())
            {
                request.ReadWriteTimeout = request.KeepAliveTimeout
                   = request.ConnectTimeout = (int)RequestTimeout.TotalMilliseconds;

                if (Proxy != null)
                    request.Proxy = Proxy;

                string inResponseString = request.Get(inUrl).ToString();
                RucaptchaResponse inResponse = JsonConvert.DeserializeObject<RucaptchaResponse>(inResponseString);

                if (inResponse.Status != 1)
                    throw new CaptchaSolvingException(inResponse.Status, this, $"Captcha error: {inResponse.Request}");

                OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha sended | ID = {inResponse.Request}"));

                RucaptchaResponse solveResponse = new RucaptchaResponse() { Status = 0, Request = CAPTCHA_NOT_READY };

                while (solveResponse.Status == 0)
                {
                    Thread.Sleep(_delay);

                    string solveResponseString = request.Get($"{Url}/res.php?key={Key}&action=get&json=1&id={inResponse.Request}").ToString();
                    solveResponse = JsonConvert.DeserializeObject<RucaptchaResponse>(solveResponseString);

                    if (solveResponse.Status == 0 && solveResponse.Request != CAPTCHA_NOT_READY)
                        throw new CaptchaSolvingException(inResponse.Status, this, $"Captcha error: {inResponse.Request}");

                    OnLogMessage?.Invoke(this, new OnLogMessageEventArgs($"Captcha result: {solveResponse.Request}"));
                }

                return solveResponse.Request;
            }
        }

        public string SolveRecaptchaV2(string siteKey, string pageUrl, bool invisible)
        {
            string inUrl = $"{Url}/in.php?" +
                           $"key={Key}" +
                           $"&method=userrecaptcha" +
                           $"&googlekey={siteKey}" +
                           $"&pageurl={System.Web.HttpUtility.UrlEncode(pageUrl)}" +
                           $"&invisible={(invisible ? "1" : "0")}" +
                           $"&json=1";

            return GetAnswer(inUrl);
        }

        public string SolveRecaptchaV2Enterprise(string siteKey, string pageUrl)
        {
            string inUrl = $"{Url}/in.php?" +
                        $"key={Key}" +
                        $"&method=userrecaptcha" +
                        $"&version=v2" +
                        $"&googlekey={siteKey}" +
                        $"&pageurl={pageUrl}" +
                        $"&enterprise=1" +
                        $"&json=1";

            return GetAnswer(inUrl);
        }

        public string SolveRecaptchaV3(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3)
        {
            string inUrl = $"{Url}/in.php?" +
                       $"key={Key}" +
                       $"&method=userrecaptcha" +
                       $"&version=v3" +
                       $"&googlekey={siteKey}" +
                       $"&pageurl={pageUrl}" +
                       $"&domain={domain}" +
                       $"&min_score={minScore.ToString(CultureInfo.InvariantCulture)}" +
                       $"&json=1";

            if (action != null)
                inUrl += $"&action={action}";

            return GetAnswer(inUrl);
        }

        public string SolveRecaptchaV3Enterprise(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3)
        {
            string inUrl = $"{Url}/in.php?" +
                          $"key={Key}" +
                          $"&method=userrecaptcha" +
                          $"&version=v3" +
                          $"&googlekey={siteKey}" +
                          $"&pageurl={pageUrl}" +
                          $"&enterprise=1" +
                          $"&domain={domain}" +
                          $"&min_score={minScore.ToString(CultureInfo.InvariantCulture)}" +
                          $"&json=1";

            if (action != null)
                inUrl += $"&action={action}";

            return GetAnswer(inUrl);
        }

        public string SolveRecaptchaV3EnterpriseProxy(string siteKey, string pageUrl, string action = null, string domain = null, double minScore = 0.3, string proxy = null, string proxyType = null)
        {
            string inUrl = $"{Url}/in.php?" +
                          $"key={Key}" +
                          $"&method=userrecaptcha" +
                          $"&version=v3" +
                          $"&googlekey={siteKey}" +
                          $"&pageurl={pageUrl}" +
                          $"&enterprise=1" +
                          $"&domain={domain}" +
                          $"&min_score={minScore.ToString(CultureInfo.InvariantCulture)}" +
                          $"&json=1";

            if (action != null)
                inUrl += $"&action={action}";

            if (proxy != null)
                inUrl += $"&action={action}";

            if (proxyType != null)
                proxyType += $"&action={action}";

            return GetAnswer(inUrl);
        }

        public string SolveHCaptcha(string siteKey, string pageUrl, bool invisible = false, string additionalData = null)
        {
            string inUrl = $"{Url}/in.php?" +
                           $"key={Key}" +
                           $"&method=hcaptcha" +
                           $"&sitekey={siteKey}" +
                           $"&pageurl={System.Web.HttpUtility.UrlEncode(pageUrl)}" +
                           $"&invisible={(invisible ? "1" : "0")}" +
                           $"&json=1";

            if (additionalData != null)
                inUrl += $"&data={additionalData}";
            
            return GetAnswer(inUrl);
        }

        public GeeTestV3CaptchaResult SolveGeeTestV3Captcha(string gt, string challenge, string pageUrl, string apiServer = null)
        {
            string inUrl = $"{Url}/in.php?" +
                    $"key={Key}" +
                    $"&method=geetest" +
                    $"&gt={gt}" +
                    $"&pageurl={System.Web.HttpUtility.UrlEncode(pageUrl)}" +
                    $"&challenge={challenge}" +
                    $"&json=1";

            if (apiServer != null)
                inUrl += $"&api_server={apiServer}";

            return new GeeTestV3CaptchaResult() { Validate = GetAnswer(inUrl) };
        }

        public GeeTestV4CaptchaResult SolveGeeTestV4Captcha(string captchaId, string pageUrl, string apiServer = null, string geetestGetLib = null, object initParametets = null)
        {
            string inUrl = $"{Url}/in.php?" +
                  $"key={Key}" +
                  $"&method=geetest_v4 " +
                  $"&captcha_id={captchaId}" +
                  $"&pageurl={System.Web.HttpUtility.UrlEncode(pageUrl)}" +
                  $"&json=1";

            if (apiServer != null)
                inUrl += $"&api_server={apiServer}";

            return new GeeTestV4CaptchaResult() { CaptchaOutput = GetAnswer(inUrl) };
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
