namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions.Web;
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
            try
            {
                var data = context.Data as JObject;
                if (data != null && data["type"].Value<string>() == EventTypeName)
                {
                    var hookEvent = data.ToObject<HookEvent>();
                    hookEvent.PayloadObject = JsonConvert.DeserializeObject<Payload>(hookEvent.EncodedPayload);

                    EnqueueAsync(context, hookEvent);
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return Task.FromResult(true);
        }
    }
}