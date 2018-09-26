namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Threading.Tasks;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;

    public class HealthCheckEventHandler : BaseLetstalkEventHandler
    {
        private const string eventTypeName = "health_check";

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            //Fanex.Logging.LogService.Info(LogHelper.BuildRequestContent(HttpContext.Current.Request.Headers, hookEvent));

            return Task.FromResult(true);
        }
    }
}