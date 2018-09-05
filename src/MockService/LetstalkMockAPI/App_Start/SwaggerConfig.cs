[assembly: System.Web.PreApplicationStartMethod(typeof(LetstalkMockAPI.SwaggerConfig), "Register")]

namespace LetstalkMockAPI
{
    using System.Web.Http;
    using Swashbuckle.Application;

    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "LetstalkMockAPI");
                    c.IncludeXmlComments(string.Format(@"{0}\bin\LetstalkMockAPI.XML",
                           System.AppDomain.CurrentDomain.BaseDirectory));
                })
                .EnableSwaggerUi(c => c.DocExpansion(DocExpansion.List));
        }
    }
}