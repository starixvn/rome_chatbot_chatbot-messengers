namespace LetstalkMockAPI.Models
{
    using System.Collections.Generic;

    public class GetContactsResult : Result
    {
        public string Tag { get; set; }

        public IEnumerable<Contact> Contacts { get; set; }
    }
}