namespace Messenger.WebHooks.Extensions
{
    using System.Web.Http;
    using Microsoft.AspNet.WebHooks.Config;

    public static class HttpConfigurationExtensions
    {
        public static void InitializeWebHooks(this HttpConfiguration config)
        {
            WebHooksConfig.Initialize(config);
        }
    }
}