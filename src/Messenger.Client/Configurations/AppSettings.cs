namespace Messenger.Client.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public interface IMessengerSettings
    {
        string ApiEndpoint { get; }

        string ApiKey { get; }

        string SecureKey { get; }

        string ChatKey { get; }

        int BotAccountId { get; }

        string BotUserId { get; }
    }

    public class LetstalkSettings : IMessengerSettings
    {
        public LetstalkSettings(
            string apiEndpoint,
            string apiKey,
            string secureKey,
            string chatKey,
            int botAccountId,
            string botUserId)
        {
            ApiEndpoint = apiEndpoint;
            ApiKey = apiKey;
            SecureKey = secureKey;
            ChatKey = chatKey;
            BotAccountId = botAccountId;
            BotUserId = botUserId;
        }

        public string ApiEndpoint { get; }

        public string ApiKey { get; }

        public string SecureKey { get; }

        public string ChatKey { get; }

        public int BotAccountId { get; }

        public string BotUserId { get; }
    }

    public static class AppSettings
    {
        /// <summary>
        /// Support for .net core
        /// </summary>
        private readonly static IConfigurationRoot configurationRoot;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static AppSettings()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var configFileName = "appsettings.json";

            if (File.Exists(Path.Combine(currentDirectory, configFileName)))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(currentDirectory)
                    .AddJsonFile(configFileName);

                configurationRoot = builder.Build();
            }

            var botId = GetConfigs("MessengerAPIClient:BotAccountId");
            var messengerBotAccountId = int.Parse(botId);
            var messengerApiEndpoint = GetConfigs("MessengerAPIClient:ApiEndpoint");
            var messengerApiKey = GetConfigs("MessengerAPIClient:ApiKey");
            var messengerSecureKey = GetConfigs("MessengerAPIClient:SecureKey");
            var messengerChatKey = GetConfigs("MessengerAPIClient:ChatKey");
            var messengerBotUserId = GetConfigs("MessengerAPIClient:BotUserId");

            Messenger = new LetstalkSettings(
                messengerApiEndpoint,
                messengerApiKey,
                messengerSecureKey,
                messengerChatKey,
                messengerBotAccountId,
                messengerBotUserId);
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

        private static string GetConfigs(string configName)
        {
            var configValue = configurationRoot == null
                ? Configs[configName]
                : configurationRoot[configName];

            return configValue ?? throw new InvalidOperationException($"Missing config for {configName}");
        }

        /// <summary>
        /// Support for .net framework
        /// </summary>
        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IMessengerSettings Messenger { get; private set; }
    }
}