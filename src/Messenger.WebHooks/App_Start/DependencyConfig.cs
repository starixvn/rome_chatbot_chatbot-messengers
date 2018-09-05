namespace Messenger.WebHooks.App_Start
{
    using Chatbot.Client.Services;
    using Fanex.Bot.Client;
    using Fanex.Logging;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using SimpleInjector;

    public static class DependencyConfig
    {
        public static void Init()
        {
            var container = new Container();

            container.RegisterSingleton<IJwtHelper, JwtHelper>();
            container.RegisterSingleton<ICryptographyHelper, CryptographyHelper>();
            container.RegisterSingleton<CommandService, CommandServiceImpl>();
            container.RegisterInstance(Logger.Log);

            container.Verify();

            DependencyResolver.SetupContainer(container);
        }
    }
}