namespace Messenger.Client.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MessageMarkup
    {
        public string Type { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public IEnumerable<MessageAction> MessageActions { get; set; }
    }
}