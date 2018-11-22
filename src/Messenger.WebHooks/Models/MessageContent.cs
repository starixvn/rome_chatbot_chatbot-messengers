namespace Messenger.WebHooks.Models
{
    using Newtonsoft.Json;

    public class MessageContent
    {
        public string Msg { get; set; }

        public Quote Quote { get; set; }

        public Postback Postback { get; set; }
    }

    public class Quote
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "account_name")]
        public string AccountName { get; set; }

        public string Content { get; set; }
    }

    public class Postback
    {
        public string Data { get; set; }
    }
}