namespace Messenger.Client.Models
{
    using Newtonsoft.Json;

    public class Message
    {
        [JsonProperty(PropertyName = "msg")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "reply_markup")]
        public MessageMarkup MessageMarkup { get; set; }
    }
}