namespace ChatR.Models
{
    using Newtonsoft.Json;

    public class Source
    {
        public string Type { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
    }
}