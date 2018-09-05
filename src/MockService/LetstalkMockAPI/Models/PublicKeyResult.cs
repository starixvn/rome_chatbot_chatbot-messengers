namespace LetstalkMockAPI.Models
{
    using Newtonsoft.Json;

    public class PublicKeyResult : Result
    {
        [JsonProperty(PropertyName = "public_key")]
        public string PublicKey { get; set; }
    }
}