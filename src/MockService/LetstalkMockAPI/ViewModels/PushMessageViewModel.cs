namespace LetstalkMockAPI.ViewModels
{
    using LetstalkMockAPI.Models;

    public class PushMessageViewModel
    {
        public string[] Receiver { get; set; }

        public Payload Payload { get; set; }
    }
}