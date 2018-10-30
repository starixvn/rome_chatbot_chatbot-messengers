namespace Messenger.Client.Models
{
    using Newtonsoft.Json;

    public class MessageAction
    {
        public string Type { get; set; }

        public string Label { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Uri { get; set; }
    }
}