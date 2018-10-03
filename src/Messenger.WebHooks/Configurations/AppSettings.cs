namespace Messenger.WebHooks.Configurations
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;

    public interface IWebhookSettings
    {
        IEnumerable<string> WhiteListIPs { get; }

        IEnumerable<string> LoggingMessengerUsernames { get; }

        bool EnabledLogAllRequest { get; }
    }

    public class WebhookSettings : IWebhookSettings
    {
        public WebhookSettings(
            string whiteListIPs,
            string loggingMessengerUsernames,
            bool enabledLogAllRequest)
        {
            WhiteListIPs = whiteListIPs.Split(',');
            LoggingMessengerUsernames = loggingMessengerUsernames.Split(',').Where(name => !string.IsNullOrWhiteSpace(name));
            EnabledLogAllRequest = enabledLogAllRequest;
        }

        public IEnumerable<string> WhiteListIPs { get; }

        public IEnumerable<string> LoggingMessengerUsernames { get; }

        public bool EnabledLogAllRequest { get; }
    }

    public static class AppSettings
    {
        static AppSettings()
        {
            var whiteListIPs = GetConfigs("Webhook:WhiteListIPs");
            var loggingMessengerUsernames = GetConfigs("Webhook:LoggingMessengerUsernames");
            var enabledLogAllRequest = GetConfigs("Webhook:EnabledLogAllRequest") == "1";

            Settings = new WebhookSettings(
                whiteListIPs,
                loggingMessengerUsernames,
                enabledLogAllRequest);
        }

#pragma warning restore S3963 // "static" fields should be initialized inline

        private static string GetConfigs(string configName)
        {
            var configValue = Configs[configName];

            return configValue ?? throw new InvalidOperationException($"Missing config for {configName}");
        }

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IWebhookSettings Settings { get; private set; }

        public static string MonitorLetstalkId => Configs["Webhook:MonitorLetstalkId"];

        public static string HeathMonitorLogCategory => Configs["Webhook:HeathMonitorLogCategory"];
    }
}