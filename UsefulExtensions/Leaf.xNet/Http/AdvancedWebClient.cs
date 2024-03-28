using System;
using System.Net;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Leaf.xNet
{
    public class AdvancedWebClient : WebClient
    {
        /// <summary>
        /// Request timeout in milliseconds. By default: 10 seconds (10 000 ms).
        /// </summary>
        public int Timeout { get; set; } = 10 * 1000;

        /// <summary>
        /// Request read-write timeout in milliseconds. By default: 10 seconds (10 000 ms).
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 10 * 1000;

        /// <summary>
        /// Decompression methods. By default: GZip and Deflate.
        /// </summary>
        public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        /// <summary>
        /// Check SSL Certificate before request. By default: all certificates allowed (false).
        /// </summary>
        public bool ServerCertificateValidation { get; set; } = false;


        protected override WebRequest GetWebRequest(Uri uri)
        {
            var webRequest = base.GetWebRequest(uri);
            if (webRequest == null)
                throw new NullReferenceException($"Null reference: unable to get instance of {nameof(WebRequest)} in {nameof(AdvancedWebClient)}.");

            webRequest.Timeout = Timeout;

            var httpWebRequest = (HttpWebRequest)webRequest;

            httpWebRequest.ReadWriteTimeout = ReadWriteTimeout;
            httpWebRequest.AutomaticDecompression = DecompressionMethods;
            if (!ServerCertificateValidation)
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            

            return webRequest;
        }
    }
}