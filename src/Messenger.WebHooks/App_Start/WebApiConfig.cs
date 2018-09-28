namespace Messenger.WebHooks
{
    using System.Web.Http;
    using Messenger.WebHooks.Extensions;
    using Messenger.WebHooks.Filters;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Filters.Add(new ExceptionFilter());
            config.InitializeWebHooks();
        }
    }
}