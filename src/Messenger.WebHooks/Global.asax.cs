namespace Messenger.WebHooks
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Http;
    using Chatbot.Client;
    using Chatbot.Client.Configurations;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.RabbitMQ;
    using Fanex.Logging.Sentry;
    using Messenger.WebHooks.App_Start;
    using Sentry;
    using Sentry.Protocol;

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

            LogManager
                   .SetDefaultLogCategory("Messenger-WebHooks")
                   .Use(new List<ILogEngine>
                   {
                       new RabbitMQLogging("host=10.18.200.103;virtualHost=/Fanex_Logging;username=fanexlogging;password=YKQrMtla7oYZGlP8apPtSw=="),
                       new SentryLogging(new SentryEngineOptions
                       {
                           Dsn = new Dsn("http://cedc74b2ae244da599820b107361e389@sentry.nexdev.net/3"),
                           GetUserInfo = () => new User
                           {
                               Username = "ricky.nguyen2"
                           }
                       })
                   });

            DependencyConfig.Init();


            ExceptionHandler.UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }
    }
}