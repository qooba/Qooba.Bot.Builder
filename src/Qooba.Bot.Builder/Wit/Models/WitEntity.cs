using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qooba.Bot.Builder.Models;

namespace Qooba.Bot.Builder.Wit.Models
{
    public class WitEntity
    {
        [JsonProperty(PropertyName = "confidence")]
        public double? Confidence { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public JToken Value { get; set; }
    }
}