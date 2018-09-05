namespace LetstalkMockAPI.Configurations
{
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static string MessengerAppUrl => Configs["MessengerAppUrl"];
    }
}