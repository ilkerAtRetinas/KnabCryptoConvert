using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Response
{
    /// <summary>
    /// A generic class for all possible conmarketcap api responses
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}
