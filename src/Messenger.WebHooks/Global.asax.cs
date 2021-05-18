namespace Messenger.WebHooks
{
    using System;
    using System.Configuration;
    using System.Web.Http;
    using Chatbot.Client;
    using Chatbot.Client.Configurations;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.RabbitMQ;
    using Messenger.WebHooks.App_Start;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var appSettings = ConfigurationManager.AppSettings;
            LogManager
                .SetDefaultLogCategory(appSettings["Fanex.Logging:DefaultCategory"])
                .Use(new RabbitMQLogging(appSettings["Fanex.Logging:RabbitMQConnectionStrings"]));

            GlobalConfiguration.Configure(WebApiConfig.Register);

            var chatbotBaseUrl = appSettings["Chatbot:BaseUrl"] ?? throw new InvalidOperationException("Missing config for Chatbot:BaseUrl");
            var chatbotApiKey = appSettings["Chatbot:ApiKey"] ?? throw new InvalidOperationException("Missing config for Chatbot:ApiKey");

            ChatbotClientManager
                .UseConfig(new BotClientConfigs(chatbotBaseUrl, chatbotApiKey))
                .UseRestClient(new Chatbot.Client.Transport.RestClient());

            DependencyConfig.Init();

            ExceptionHandler.UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }
    }
}