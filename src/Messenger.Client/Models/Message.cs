namespace Messenger.Client.Models
{
    using System;
    using Newtonsoft.Json;

    public class Message
    {
        [JsonProperty(PropertyName = "msg")]
        public string Content { get; set; }

        [JsonIgnore]
        public DateTime? ExpiredTime { get; set; }

        [JsonProperty(PropertyName = "reply_markup", NullValueHandling = NullValueHandling.Ignore)]
        public MessageMarkup MessageMarkup { get; set; }
    }
}