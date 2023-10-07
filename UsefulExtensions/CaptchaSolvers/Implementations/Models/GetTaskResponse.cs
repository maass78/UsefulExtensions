using Newtonsoft.Json;

namespace UsefulExtensions.CaptchaSolvers.Implementations.Models
{
    internal class GetTaskResponse<T>
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

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
}