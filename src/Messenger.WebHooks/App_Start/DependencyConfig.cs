namespace Messenger.WebHooks.App_Start
{
    using Chatbot.Client.Services;
    using Fanex.Logging;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.Configurations;
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
            container.RegisterInstance(AppSettings.Settings);
            container.RegisterSingleton<IMessengerSettings>(() => new MessengerSettings());

            container.Verify();

            DependencyResolver.SetupContainer(container);
        }
    }
}