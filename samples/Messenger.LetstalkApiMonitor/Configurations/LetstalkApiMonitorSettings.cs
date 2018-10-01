namespace Messenger.LetstalkApiMonitor.Configurations
{
    public class LetstalkApiMonitorSettings
    {
        public int WorkingIntervalTime { get; set; }

        public string AccountId { get; set; }

        public int WebhookLogCategoryId { get; set; }

        public string SkypeConversationId { get; set; }

        public bool IsProduction { get; set; }

        public int VerifyLogJobStartAfterSecond { get; set; }
    }
}