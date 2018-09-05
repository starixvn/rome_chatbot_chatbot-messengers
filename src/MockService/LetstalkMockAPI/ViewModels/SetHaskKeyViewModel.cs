using Newtonsoft.Json;

namespace LetstalkMockAPI.ViewModels
{
    public class SetHashKeyViewModel
    {
        [JsonProperty(PropertyName = "hash_key")]
        public string HashKey { get; set; }

        [JsonProperty(PropertyName = "hash_id")]
        public long HashId { get; set; }
    }
}
