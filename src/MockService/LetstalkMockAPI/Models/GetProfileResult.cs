namespace LetstalkMockAPI.Models
{
    using Newtonsoft.Json;

    public class GetProfileResult : Result
    {
        [JsonProperty(PropertyName = "avatar_url")]
        public string AvartaUrl { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }
    }
}