using Newtonsoft.Json;

namespace UsefulExtensions.CaptchaSolvers.Implementations.Models
{
    internal class CreateTaskResponse
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("taskId")]
        public long TaskId { get; set; }
    }
}
