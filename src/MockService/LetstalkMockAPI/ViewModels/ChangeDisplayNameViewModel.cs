namespace LetstalkMockAPI.ViewModels
{
    using Newtonsoft.Json;

    public class ChangeDisplayNameViewModel
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }
    }
}