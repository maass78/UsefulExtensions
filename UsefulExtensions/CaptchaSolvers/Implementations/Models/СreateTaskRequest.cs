using Newtonsoft.Json;

namespace UsefulExtensions.CaptchaSolvers.Implementations.Models
{
    internal class СreateTaskRequest<T>
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("task")]
        public T Task { get; set; }
    }
}
