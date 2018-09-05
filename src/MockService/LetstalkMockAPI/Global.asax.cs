using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using LetstalkMockAPI.App_Start;

namespace LetstalkMockAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
