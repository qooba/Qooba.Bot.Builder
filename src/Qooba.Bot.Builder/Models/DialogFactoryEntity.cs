using Newtonsoft.Json.Linq;

namespace Qooba.Bot.Builder.Models
{
    public class DialogFactoryEntity
    {
        public string Key { get; set; }

        public double? Confidence { get; set; }

        public string Type { get; set; }

        public JToken Value { get; set; }
    }
}