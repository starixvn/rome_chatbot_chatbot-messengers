namespace LetstalkMockAPI.Models
{
    using System;

    public class ChatMessage
    {
        public int AccountId { get; set; }

        public string Username { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}