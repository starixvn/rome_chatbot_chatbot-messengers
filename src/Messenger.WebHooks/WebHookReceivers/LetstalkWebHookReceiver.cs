namespace Messenger.WebHooks.WebHookReceivers
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Controllers;
    using Fanex.Logging.Extensions.Web;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Configurations;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json.Linq;

    public class LetstalkWebHookReceiver : WebHookReceiver
    {
        internal const string ReceiverName = "Letstalk";

        public override string Name => ReceiverName;

        private readonly IJwtHelper jwtHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly IWebhookSettings webhookSettings;

        public LetstalkWebHookReceiver() : this(null, null, null)
        {
        }

        public LetstalkWebHookReceiver(
            IJwtHelper jwtHelper,
            IMessengerSettings messengerSettings,
            IWebhookSettings webhookSettings)
        {
            this.jwtHelper = jwtHelper ?? DependencyResolver.GetInstance<IJwtHelper>();
            this.messengerSettings = messengerSettings ?? Client.Configurations.AppSettings.Messenger;
            this.webhookSettings = webhookSettings ?? Configurations.AppSettings.Settings;
        }

        public override async Task<HttpResponseMessage> ReceiveAsync(
            string id,
            HttpRequestContext context,
            HttpRequestMessage request)
        {
            ValidateRequest(id, context, request);

            ExceptionHandler.Handle(new Exception("Received Webhook Event"), System.Diagnostics.TraceEventType.Information);

            if (webhookSettings.WhiteListIPs.Any()
                && HttpContext.Current != null
                && HttpContext.Current.Request != null)
            {
                var userIP = GetClientIP(HttpContext.Current.Request.ServerVariables);
                if (!webhookSettings
                    .WhiteListIPs
                    .Any(ip => userIP.Contains(ip)))
                {
                    return await Task.FromResult(request.CreateErrorResponse(HttpStatusCode.Forbidden, "IP Restricted"));
                }
            }

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
            var rawData = jwtHelper.Decode(messengerSettings.SecureKey, token);

            return JObject.Parse(rawData);
        }

        private static string GetClientIP(NameValueCollection serverVariables)
        {
            var clientIP = serverVariables["HTTP_X_FORWARDED_FOR"]
                ?? serverVariables["REMOTE_ADDR"]
                ?? string.Empty;

            return string.IsNullOrEmpty(clientIP) || clientIP.Equals("::1")
                ? "127.0.0.1"
                : clientIP;
        }
    }
}