namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Threading.Tasks;
    using Messenger.Client;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;

    public class AskKeyEventHandler : BaseLetstalkEventHandler
    {
        private const string eventTypeName = "ask_key";

        public AskKeyEventHandler()
        {
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            ////Fanex.Logging.LogService.Info(LogHelper.BuildRequestContent(HttpContext.Current.Request.Headers, hookEvent));

            ////var result = messengerClient.Push(
            ////                1,
            ////                new List<string>
            ////                {
            ////                    hookEvent.Source.UserId
            ////                },
            ////                new Payload
            ////                {
            ////                    Id = 1,
            ////                    From = 0,
            ////                    UserId = hookEvent.Source.UserId,
            ////                    HashContent = $"Receive Message From: {hookEvent.Source.UserId}, Hook Event Type: {eventTypeName}"
            ////                });

            ////Fanex.Logging.LogService.Info($"Request: Push, response: {JsonConvert.SerializeObject(result)}");

            return Task.FromResult(true);
        }
    }
}