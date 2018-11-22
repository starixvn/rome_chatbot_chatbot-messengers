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
#pragma warning disable S3996 // URI properties should not be strings
        public string Uri { get; set; }
#pragma warning restore S3996 // URI properties should not be strings
    }
}