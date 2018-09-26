namespace Messenger.LetstalkApiMonitor
{
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using Messenger.LetstalkApiMonitor.Configurations;
    using Messenger.LetstalkApiMonitor.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // configure settings
            services.Configure<LetstalkApiMonitorSettings>(Configuration);
            services.AddOptions();

            // configure Fanex.Logging
            LogManager
                    .SetDefaultLogCategory(Configuration["Fanex.Logging:DefaultCategory"])
                    .Use(new RabbitMQLogging(Configuration["Fanex.Logging:RabbitMQConnectionStrings"]));
            services.AddSingleton(Logger.Log);

            //configure background task
            services.AddHostedService<LetstalkApiMonitorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Letstalk API Monitor has been started!");
            });
        }
    }
}