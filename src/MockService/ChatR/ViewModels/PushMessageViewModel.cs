using ChatR.Models;

namespace ChatR.ViewModels
{
    public class PushMessageViewModel
    {
        public string[] Receiver { get; set; }

        public Payload Payload { get; set; }
    }
}