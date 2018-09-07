namespace Messenger.WebHooks.Site.Sample.Configurations
{
    using System.Collections.Specialized;
    using System.Configuration;
    using Messenger.Client.Configurations;

    public static class AppSettings
    {
#pragma warning disable S3963 // "static" fields should be initialized inline

        static AppSettings()
        {
            var botId = Configs["MessengerAPIClient:BotAccountId"];
            var messengerBotAccountId = int.Parse(botId);
            var messengerApiEndpoint = Configs["MessengerAPIClient:ApiEndpoint"];
            var messengerApiKey = Configs["MessengerAPIClient:ApiKey"];
            var messengerSecureKey = Configs["MessengerAPIClient:SecureKey"];
            var messengerChatKey = Configs["MessengerAPIClient:ChatKey"];
            var messengerBotUserId = Configs["MessengerAPIClient:BotUserId"];

            Messenger = new LetstalkSettings(
                messengerApiEndpoint,
                messengerApiKey,
                messengerSecureKey,
                messengerChatKey,
                messengerBotAccountId,
                messengerBotUserId);
        }

#pragma warning restore S3963 // "static" fields should be initialized inline

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IMessengerSettings Messenger { get; private set; }
    }
}