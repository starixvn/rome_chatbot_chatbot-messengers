namespace LetstalkMockAPI.ViewModels
{
    using Newtonsoft.Json;

    public class SetKeyToContactViewModel
    {
        [JsonProperty(PropertyName = "encryption_key")]
        public string EncryptionKey { get; set; }

        [JsonProperty(PropertyName = "hash_id")]
        public long HashId { get; set; }
    }
}