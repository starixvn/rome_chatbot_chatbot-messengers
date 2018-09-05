namespace Messenger.WebHooks.Models
{
    using Newtonsoft.Json;

    public class HookEvent
    {
        public string Type { get; set; }

        public long TimeStamp { get; set; }

        [JsonProperty(PropertyName = "payload")]
        public string EncodedPayload { get; set; }

        public Payload PayloadObject { get; set; }

        public EventSource Source { get; set; }
    }
}