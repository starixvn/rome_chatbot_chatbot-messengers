namespace Messenger.WebHooks.WebHookReceivers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using Fanex.Logging.Extensions.Web;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.App_Start;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json.Linq;

    public class LetstalkWebHookReceiver : WebHookReceiver
    {
        internal const string ReceiverName = "Letstalk";

        public override string Name => ReceiverName;

        private readonly IJwtHelper jwtHelper;
        private readonly string SecureKey = ConfigurationManager.AppSettings["MessengerAPIClient:SecureKey"];

        public LetstalkWebHookReceiver() : this(null)
        {
        }

        public LetstalkWebHookReceiver(
            IJwtHelper jwtHelper)
        {
            this.jwtHelper = jwtHelper ?? DependencyResolver.GetInstance<IJwtHelper>();
        }

        public override async Task<HttpResponseMessage> ReceiveAsync(
            string id,
            HttpRequestContext context,
            HttpRequestMessage request)
        {
            ValidateRequest(id, context, request);

            ExceptionHandler.Handle(new Exception("Received Webhook Event"), System.Diagnostics.TraceEventType.Information);

            if (request.Method == HttpMethod.Post)
            {
                return await HandleEvent(id, context, request);
            }

            /// TODO: Creates a response to a WebHook verification GET request, should implement this for testing alive webhook
            ////else if (request.Method == HttpMethod.Get)
            ////{
            ////    return await WebHookVerification(id, request);
            ////}

            return CreateBadMethodResponse(request);
        }

        private async Task<HttpResponseMessage> HandleEvent(string id, HttpRequestContext context, HttpRequestMessage request)
        {
            var token = await request.Content.ReadAsStringAsync();

            // Read the request entity body
            var data = VerifyAndReadBodyData(token);

            // TODO: Get the event actions, currently we dont have term action, I just init an empty enumerable
            var actions = Enumerable.Empty<string>();

            // Call registered handlers
            return await ExecuteWebHookAsync(id, context, request, actions, data);
        }

        private static void ValidateRequest(string id, HttpRequestContext context, HttpRequestMessage request)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

        private JObject VerifyAndReadBodyData(string token)
        {
            var rawData = jwtHelper.Decode(SecureKey, token);

            return JObject.Parse(rawData);
        }
    }
}