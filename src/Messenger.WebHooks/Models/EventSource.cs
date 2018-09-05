namespace Messenger.WebHooks.Models
{
    using Newtonsoft.Json;

    public class EventSource
    {
        public string Type { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
    }
}