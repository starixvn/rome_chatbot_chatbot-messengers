[assembly: Microsoft.Owin.OwinStartup(typeof(ChatR.Startup))]

namespace ChatR
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}