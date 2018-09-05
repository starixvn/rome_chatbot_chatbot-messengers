namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Configuration;
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Fanex.Logging;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        private readonly CommandService commandService;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly ILogger logger;
        private const string eventTypeName = "message";

        public MessageEventHandler() : this(null, null, null)
        {
        }

        public MessageEventHandler(
            CommandService commandService,
            ICryptographyHelper cryptographyHelper = null,
            ILogger logger = null)
        {
            this.commandService = commandService ?? DependencyResolver.GetInstance<CommandService>();
            this.cryptographyHelper = cryptographyHelper ?? DependencyResolver.GetInstance<ICryptographyHelper>();
            this.logger = logger ?? Logger.Log;
        }

        protected override string EventTypeName => eventTypeName;

        protected override async Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            var messageContent = cryptographyHelper.Decrypt(
                hookEvent.PayloadObject.HashContent,
                ConfigurationManager.AppSettings["MessengerAPIClient:ChatKey"],
                hookEvent.PayloadObject.HashContentKey);

            var command = JsonConvert.DeserializeObject<MessageContent>(messageContent).Msg;

            logger.Info(JsonConvert.SerializeObject(new
            {
                command,
                MessengerType,
                hookEvent
            }, Formatting.Indented));

            await commandService.Run(command, MessengerType, hookEvent.Source.UserId);
        }
    }
}