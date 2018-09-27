namespace Messenger.LetstalkApiMonitor.Services
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Messenger.Client;
    using Messenger.LetstalkApiMonitor.Configurations;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class LetstalkApiMonitorService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IMessengerClient letstalkClient;
        private readonly LetstalkApiMonitorSettings settings;

        public LetstalkApiMonitorService(
            ILogger logger,
            IMessengerClient letstalkClient,
            IOptions<LetstalkApiMonitorSettings> settings)
        {
            this.logger = logger;
            this.letstalkClient = letstalkClient;
            this.settings = settings?.Value
                ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Debug($"Letstalk API Monitor Service is starting.");
            stoppingToken.Register(() => logger.Debug($"#1 Letstalk API Monitor Service is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.Debug($"Letstalk API Monitor Service is doing background work.");

                try
                {
                    var result = await letstalkClient.PushAsync(settings.AccountId, $"Health check message at {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}");

                    var message = new StringBuilder();
                    message.AppendLine("#LetstalkAPI : " + (result.IsOk ? "OK" : "Failed") + " at " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                    message.Append(JsonConvert.SerializeObject(result, Formatting.Indented));

                    if (result.IsOk)
                    {
                        logger.Info(message.ToString());
                    }
                    else
                    {
                        logger.Error(message.ToString());
                    }
                }
                catch (Exception ex)
                {
                    var message = new StringBuilder();
                    message.AppendLine($"#LetstalkAPI : Failed on {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}");
                    message.Append(ex.Message);

                    logger.Error(message.ToString(), ex);
                }
                finally
                {
                    await Task.Delay(settings.WorkingIntervalTime, stoppingToken);
                }
            }

            await Task.CompletedTask;
        }
    }
}