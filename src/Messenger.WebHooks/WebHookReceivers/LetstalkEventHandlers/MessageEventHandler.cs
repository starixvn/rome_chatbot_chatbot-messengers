namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Web;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Configurations;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        private readonly string UnrecognizedCommand = "Unrecognized Command";
        private readonly CommandService commandService;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly ILogger logger;
        private readonly IMessengerSettings messengerSettings;
        private readonly IWebhookSettings webhookSettings;
        private const string eventTypeName = "message";

        public MessageEventHandler() : this(null, null, null, null, null)
        {
        }

        public MessageEventHandler(
            CommandService commandService,
            ICryptographyHelper cryptographyHelper = null,
            ILogger logger = null,
            IMessengerSettings messengerSettings = null,
            IWebhookSettings webhookSettings = null)
        {
            this.commandService = commandService ?? DependencyResolver.GetInstance<CommandService>();
            this.cryptographyHelper = cryptographyHelper ?? DependencyResolver.GetInstance<ICryptographyHelper>();
            this.logger = logger ?? Logger.Log;
            this.messengerSettings = messengerSettings ?? Client.Configurations.AppSettings.Messenger;
            this.webhookSettings = webhookSettings ?? Configurations.AppSettings.Settings;
        }

        protected override string EventTypeName => eventTypeName;

        protected override async Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            try
            {
                var rawMessage = cryptographyHelper.Decrypt(
                    hookEvent.PayloadObject.HashContent,
                    messengerSettings.ChatKey,
                    hookEvent.PayloadObject.HashContentKey);

                var message = JsonConvert.DeserializeObject<MessageContent>(rawMessage);

                if (string.IsNullOrWhiteSpace(message.Msg))
                {
                    message.Msg = UnrecognizedCommand;
                }

                LogEvent(hookEvent, message, rawMessage);

                if (UserIsMonitoredUser(hookEvent))
                {
                    return;
                }

                await logger.InfoAsync(rawMessage);

                await commandService.Run(message.Msg, message.Postback?.Data, MessengerType, hookEvent.Source.UserId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        private static bool UserIsMonitoredUser(HookEvent hookEvent)
            => hookEvent.PayloadObject.UserId == Configurations.AppSettings.MonitorLetstalkId
                                || hookEvent.PayloadObject.Receiver == Configurations.AppSettings.MonitorLetstalkId;

        private void LogEvent(HookEvent hookEvent, MessageContent message, string rawMessage)
        {
            var jsonMessage = JsonConvert.SerializeObject(
                new
                {
                    message,
                    MessengerType,
                    hookEvent,
                    rawMessage
                },
                Formatting.Indented);

            if (IsMessageFromMonitorLetstalkUser(hookEvent, jsonMessage))
            {
                return;
            }


            if (IsIgnoredUser(hookEvent))
            {
                return;
            }

            logger.Info(jsonMessage);
        }

        private bool IsIgnoredUser(HookEvent hookEvent)
            => webhookSettings.LoggingMessengerUsernames.Any()
                            && !string.IsNullOrWhiteSpace(hookEvent.PayloadObject.UserId)
                            && !webhookSettings.LoggingMessengerUsernames.Contains(hookEvent.PayloadObject.UserId);

        private bool IsMessageFromMonitorLetstalkUser(HookEvent hookEvent, string jsonMessage)
        {
            if (hookEvent.PayloadObject.UserId == Configurations.AppSettings.MonitorLetstalkId
                || hookEvent.PayloadObject.Receiver == Configurations.AppSettings.MonitorLetstalkId)
            {
                var logMessage = new StringBuilder();
                logMessage.AppendLine($"#LetstalkWebhook : OK at {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}");
                logMessage.Append(jsonMessage);

                logger.Info(logMessage.ToString(), Configurations.AppSettings.HeathMonitorLogCategory);
                return true;
            }

            return false;
        }
    }
}