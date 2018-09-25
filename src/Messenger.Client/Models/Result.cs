[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Messenger.Client.Tests")]

namespace Messenger.Client.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Result
    {
        public int Status { get; set; }

        public bool IsOk => Status == 1;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }
    }

    internal class TagListResult : Result
    {
        public List<string> Tags { get; set; }
    }

    internal class ContactListResult : Result
    {
        public List<Contact> Contacts { get; set; }
    }

    internal class PublicKeyResult : Result
    {
        [JsonProperty(PropertyName = "public_key")]
        public string PublicKey { get; set; }
    }

    internal class ProfileResult : Result
    {
        private const int AccountDoesNotExistStatus = 21;

        [JsonProperty(PropertyName = "avatar_url")]
#pragma warning disable S3996 // URI properties should not be strings
        public string AvatarUrl { get; set; }

#pragma warning restore S3996 // URI properties should not be strings

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        public bool IsAccountNotExistent => Status == AccountDoesNotExistStatus;
    }
}