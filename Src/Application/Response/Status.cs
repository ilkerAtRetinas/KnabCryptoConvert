using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Response
{
    /// <summary>
    /// Class to hold the information regarding to the coinmarket cap api call result status
    /// </summary>
    public class Status
    {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("error_code")]
        public long ErrorCode { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("elapsed")]
        public long Elapsed { get; set; }

        [JsonProperty("credit_count")]
        public long CreditCount { get; set; }
    }
}
