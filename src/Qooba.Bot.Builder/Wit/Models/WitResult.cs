using Newtonsoft.Json;
using System.Collections.Generic;

namespace Qooba.Bot.Builder.Wit.Models
{
    public class WitResult
    {
        [JsonProperty(PropertyName = "msg_id")]
        public string MessageId { get; set; }

        [JsonProperty(PropertyName = "_text")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "entities")]
        public IDictionary<string, IList<WitEntity>> Entities { get; set; }
    }
}
