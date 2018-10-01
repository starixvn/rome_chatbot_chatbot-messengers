namespace Messenger.LetstalkApiMonitor
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using Fanex.Bot.Client;
    using Fanex.Bot.Client.Configuration;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.LetstalkApiMonitor.Configurations;
    using Messenger.LetstalkApiMonitor.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;

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
            services.AddSingleton<ISchedulerFactory>(
                    new StdSchedulerFactory(new NameValueCollection
                    {
                        { "quartz.serializer.type", "binary" }
                    }));

            var letstalkSetting = new LetstalkSettings(
                Configuration["MessengerAPIClient:ApiEndpoint"],
                Configuration["MessengerAPIClient:ApiKey"],
                Configuration["MessengerAPIClient:SecureKey"],
                Configuration["MessengerAPIClient:ChatKey"],
                int.Parse(Configuration["MessengerAPIClient:BotAccountId"]),
                Configuration["MessengerAPIClient:BotUserId"]);

            services.AddSingleton<IMessengerClient>(new LetstalkClient(letstalkSetting));
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBotConnector, BotConnector>();
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

            var connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            DbSettingProviderManager
                .StartNewSession()
                .UseConnectionStrings(connections)
                .UseDefaultDbSettingProvider(
                    Configuration.GetSection("StoreProcedureDirectory").Value ??
                    throw new InvalidOperationException("Missing StoreProcedureDirectory config"))
                .Run();

            BotClientManager.UseConfig(new BotSettings(
                  new Uri(Configuration.GetSection("FanexBotClient:BotServiceUrl").Value),
                  Configuration.GetSection("FanexBotClient:ClientId").Value,
                  Configuration.GetSection("FanexBotClient:ClientPassword").Value));
        }
    }
}