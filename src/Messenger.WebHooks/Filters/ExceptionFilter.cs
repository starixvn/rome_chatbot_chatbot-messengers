namespace Messenger.WebHooks.Filters
{
    using System.Web.Http.Filters;
    using Chatbot.Client.Models;
    using Fanex.Logging.Extensions.Web;

    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);

            if (actionExecutedContext.Exception is ChatbotClientException)
            {
                var chatbotClientException = actionExecutedContext.Exception as ChatbotClientException;
                if (chatbotClientException.IsServiceNotAvailable)
                {
                    return;
                }
            }

            ExceptionHandler.Handle(actionExecutedContext.Exception);
        }
    }
}