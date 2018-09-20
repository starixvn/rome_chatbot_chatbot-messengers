namespace Messenger.WebHooks.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;

    public interface IWebhookSettings
    {
        IEnumerable<string> WhiteListIPs { get; }

        IEnumerable<string> LoggingMessengerUsernames { get; }
    }

    public class WebhookSettings : IWebhookSettings
    {
        public WebhookSettings(
            string whiteListIPs,
            string loggingMessengerUsernames)
        {
            WhiteListIPs = whiteListIPs.Split(',');
            LoggingMessengerUsernames = loggingMessengerUsernames.Split(',');
        }

        public IEnumerable<string> WhiteListIPs { get; }

        public IEnumerable<string> LoggingMessengerUsernames { get; }
    }

    public static class AppSettings
    {
        static AppSettings()
        {
            var whiteListIPs = GetConfigs("Webhook:WhiteListIPs");
            var loggingMessengerUsernames = GetConfigs("Webhook:LoggingMessengerUsernames");

            Settings = new WebhookSettings(
                whiteListIPs,
                loggingMessengerUsernames);
        }

#pragma warning restore S3963 // "static" fields should be initialized inline

        private static string GetConfigs(string configName)
        {
            var configValue = Configs[configName];

            return configValue ?? throw new InvalidOperationException($"Missing config for {configName}");
        }

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IWebhookSettings Settings { get; private set; }
    }
}