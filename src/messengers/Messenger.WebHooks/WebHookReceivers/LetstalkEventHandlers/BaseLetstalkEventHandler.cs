namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Threading.Tasks;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public abstract class BaseLetstalkEventHandler : WebHookQueueHandler
    {
        protected const int MessengerType = 1;

        protected BaseLetstalkEventHandler()
        {
            Receiver = LetstalkWebHookReceiver.ReceiverName;
        }

        protected abstract string EventTypeName { get; }

        protected abstract Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent);

        public override Task EnqueueAsync(WebHookQueueContext context)
        {
            var data = context.Data as JObject;
            if (data != null && data["type"].Value<string>() == EventTypeName)
            {
                var hookEvent = data.ToObject<HookEvent>();
                hookEvent.PayloadObject = JsonConvert.DeserializeObject<Payload>(hookEvent.EncodedPayload);

                EnqueueAsync(context, hookEvent);
            }

            return Task.FromResult(true);
        }
    }
}