namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
                var messageContent = cryptographyHelper.Decrypt(
                    hookEvent.PayloadObject.HashContent,
                    messengerSettings.ChatKey,
                    hookEvent.PayloadObject.HashContentKey);

                ////if (!hookEvent.PayloadObject.Type.Equals(MessageType.Text))
                ////{
                ////    return;
                ////}

                var message = JsonConvert.DeserializeObject<MessageContent>(messageContent).Msg;

                LogEvent(hookEvent, message);

                if (hookEvent.PayloadObject.UserId == Configurations.AppSettings.MonitorLetstalkId
                    || hookEvent.PayloadObject.Receiver == Configurations.AppSettings.MonitorLetstalkId)
                {
                    return;
                }

                var contents = message.Split(' ');
                var command = contents[0];
                var parameter = contents.Count() > 1 ? contents[1] : string.Empty;

                await commandService.Run(command, parameter, MessengerType, hookEvent.Source.UserId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        private void LogEvent(HookEvent hookEvent, string command)
        {
            var jsonMessage = JsonConvert.SerializeObject(
                new
                {
                    command,
                    MessengerType,
                    hookEvent
                },
                Formatting.Indented);

            if (hookEvent.PayloadObject.UserId == Configurations.AppSettings.MonitorLetstalkId
                || hookEvent.PayloadObject.Receiver == Configurations.AppSettings.MonitorLetstalkId)
            {
                var message = new StringBuilder();
                message.AppendLine($"#LetstalkWebhook : OK at {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}");
                message.Append(jsonMessage);

                logger.Info(message.ToString(), Configurations.AppSettings.HeathMonitorLogCategory);
                return;
            }

            if (webhookSettings.LoggingMessengerUsernames.Any()
                && !string.IsNullOrWhiteSpace(hookEvent.PayloadObject.UserId)
                && !webhookSettings.LoggingMessengerUsernames.Contains(hookEvent.PayloadObject.UserId))
            {
                return;
            }

            logger.Info(jsonMessage);
        }
    }
}