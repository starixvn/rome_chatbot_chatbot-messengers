namespace Messenger.WebHooks
{
    using System;
    using System.Configuration;
    using System.Web.Http;
    using Chatbot.Client;
    using Chatbot.Client.Configurations;
    using Fanex.Logging.Extensions.Web;
    using Messenger.WebHooks.App_Start;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var chatbotBaseUrl = ConfigurationManager.AppSettings["Chatbot:BaseUrl"] ?? throw new InvalidOperationException("Missing config for Chatbot:BaseUrl");
            var chatbotApiKey = ConfigurationManager.AppSettings["Chatbot:ApiKey"] ?? throw new InvalidOperationException("Missing config for Chatbot:ApiKey");

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