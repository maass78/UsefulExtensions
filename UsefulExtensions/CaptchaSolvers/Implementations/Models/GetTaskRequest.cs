using Newtonsoft.Json;

namespace UsefulExtensions.CaptchaSolvers.Implementations.Models
{
    internal class GetTaskRequest
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("taskId")]
        public long TaskId { get; set; }
    }
}
