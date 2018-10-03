using Microsoft.Extensions.Configuration;

namespace Messenger.WebHooks.Configurations
{
    public interface IMessengerSettings
    {
        string GetSecureKey(string messenger, string botId);

        string GetChatKey(string messenger, string botId);
    }

    public class MessengerSettings : IMessengerSettings
    {
        private readonly IConfigurationRoot configuration;

        public MessengerSettings()
            : this(null)
        {
        }

        internal MessengerSettings(IConfigurationRoot configuration)
        {
            this.configuration = configuration ??
                new ConfigurationBuilder()
                .AddJsonFile("messengersettings.json")
                .Build();
        }

        public string GetSecureKey(string messenger, string botId)
            => configuration.GetSection($"{messenger}:{botId}:SecureKey").Value;

        public string GetChatKey(string messenger, string botId)
            => configuration.GetSection($"{messenger}:{botId}:ChatKey").Value;
    }
}