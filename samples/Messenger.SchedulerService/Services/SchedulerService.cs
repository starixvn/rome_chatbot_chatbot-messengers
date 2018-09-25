namespace Messenger.SchedulerService.Services
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.SchedulerService.Configurations;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class SchedulerService : BackgroundService
    {
        private readonly ILogger logger;

        private readonly SchedulerServiceSettings settings;
        private readonly LetstalkClient letstalkClient;

        public SchedulerService(
            IOptions<SchedulerServiceSettings> settings,
            ILogger logger)
        {
            this.settings = settings?.Value
                ?? throw new ArgumentNullException(nameof(settings));

            this.logger = logger;
            this.letstalkClient = new LetstalkClient(AppSettings.Messenger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Debug($"Messenger Scheduler Service is starting.");
            stoppingToken.Register(() => logger.Debug($"#1 Messenger Scheduler Service is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.Debug($"Messenger Scheduler Service is doing background work.");

                try
                {
                    var result = await letstalkClient.PushAsync(settings.AccountId, "test message 1");

                    var message = new StringBuilder();
                    message.AppendLine("#LetstalkAPI : " + (result.IsOk ? "Ok" : "Failed"));
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
                    message.AppendLine("#LetstalkAPI : Failed");
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