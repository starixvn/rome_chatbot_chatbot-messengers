namespace Messenger.Client.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

#if NETSTANDARD2_0
    using System.IO;
    using Microsoft.Extensions.Configuration;
#endif

    public interface IMessengerSettings
    {
        string ApiEndpoint { get; }

        string ApiKey { get; }

        string SecureKey { get; }

        string ChatKey { get; }

        int BotAccountId { get; }

        string BotUserId { get; }

        int ChatKeyVersion { get; }

        int BusinessAccountType { get; }

        int EncryptionMode { get; }
    }

    public class LetstalkSettings : IMessengerSettings
    {
#pragma warning disable S107 // Methods should not have too many parameters
        public LetstalkSettings(
            string apiEndpoint,
            string apiKey,
            string secureKey,
            string chatKey,
            int botAccountId,
            string botUserId,
            int chatKeyVersion,
            int businessAccountType,
            int encryptionMode)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            ApiEndpoint = apiEndpoint;
            ApiKey = apiKey;
            SecureKey = secureKey;
            ChatKey = chatKey;
            BotAccountId = botAccountId;
            BotUserId = botUserId;
            ChatKeyVersion = chatKeyVersion;
            BusinessAccountType = businessAccountType;
            EncryptionMode = encryptionMode;
        }

        public string ApiEndpoint { get; }

        public string ApiKey { get; }

        public string SecureKey { get; }

        public string ChatKey { get; }

        public int BotAccountId { get; }

        public string BotUserId { get; }

        public int ChatKeyVersion { get; }

        public int BusinessAccountType { get; }

        public int EncryptionMode { get; }
    }

    public static class AppSettings
    {
#if NETSTANDARD2_0
        private readonly static IConfigurationRoot configurationRoot;
#endif

#pragma warning disable S3963 // "static" fields should be initialized inline
        static AppSettings()
        {
#if NETSTANDARD2_0
            var currentDirectory = Directory.GetCurrentDirectory();
            const string configFileName = "appsettings.json";

            if (File.Exists(Path.Combine(currentDirectory, configFileName)))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(currentDirectory)
                    .AddJsonFile(configFileName);

                configurationRoot = builder.Build();
            }
#endif

            var botId = GetConfigs("MessengerAPIClient:BotAccountId");
            var messengerBotAccountId = int.Parse(botId);
            var messengerApiEndpoint = GetConfigs("MessengerAPIClient:ApiEndpoint");
            var messengerApiKey = GetConfigs("MessengerAPIClient:ApiKey");
            var messengerSecureKey = GetConfigs("MessengerAPIClient:SecureKey");
            var messengerChatKey = GetConfigs("MessengerAPIClient:ChatKey");
            var messengerBotUserId = GetConfigs("MessengerAPIClient:BotUserId");
            var chatKeyVersion = int.Parse(GetConfigs("MessengerAPIClient:ChatKeyVersion"));
            var businessAccountType = int.Parse(GetConfigs("MessengerAPIClient:BusinessAccountType"));
            var encryptionMode = int.Parse(GetConfigs("MessengerAPIClient:EncryptionMode"));

            Messenger = new LetstalkSettings(
                messengerApiEndpoint,
                messengerApiKey,
                messengerSecureKey,
                messengerChatKey,
                messengerBotAccountId,
                messengerBotUserId,
                chatKeyVersion,
                businessAccountType,
                encryptionMode);
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

        private static string GetConfigs(string configName)
        {
#if NETSTANDARD2_0
            var configValue = configurationRoot[configName];
#else
            var configValue = Configs[configName];
#endif
            return configValue ?? throw new InvalidOperationException($"Missing config for {configName}");
        }

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IMessengerSettings Messenger { get; private set; }
    }
}