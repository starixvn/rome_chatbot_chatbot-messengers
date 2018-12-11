namespace Messenger.LetstalkApiMonitor.Services
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Messenger.Client;
    using Messenger.LetstalkApiMonitor.Configurations;
    using Messenger.LetstalkApiMonitor.Jobs;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using ONEbook.UM.Services;
    using Quartz;

    public class LetstalkApiMonitorService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IMessengerClient letstalkClient;
        private readonly LetstalkApiMonitorSettings settings;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IMaintenanceService maintenanceService;

        public LetstalkApiMonitorService(
            ILogger logger,
            IMessengerClient letstalkClient,
            IOptions<LetstalkApiMonitorSettings> settings,
            ISchedulerFactory schedulerFactory,
            IMaintenanceService maintenanceService)
        {
            this.logger = logger;
            this.letstalkClient = letstalkClient;
            this.settings = settings?.Value
                ?? throw new ArgumentNullException(nameof(settings));
            this.schedulerFactory = schedulerFactory;
            this.maintenanceService = maintenanceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Debug($"Letstalk API Monitor Service is starting.");
            stoppingToken.Register(() => logger.Debug($"#1 Letstalk API Monitor Service is stopping."));
            var scheduler = await schedulerFactory.GetScheduler();
            var countSent = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                var healthCheckMessage = $"Health check message at {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}";

                try
                {
                    var underMaintenance = await maintenanceService.GetUnderMaintenanceTime();

                    if (underMaintenance.IsUnderMaintenanceTime)
                    {
                        logger.Info("Under maintenance");
                        await Task.Delay(settings.WorkingIntervalTime, stoppingToken);
                        continue;
                    }

                    var result = await letstalkClient.PushAsync(settings.AccountId, healthCheckMessage);

                    var message = new StringBuilder();
                    message.AppendLine("#LetstalkAPI : " + (result.IsOk ? "OK" : "Failed") + " at " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                    message.AppendLine(healthCheckMessage);
                    message.Append("Return Result: " + JsonConvert.SerializeObject(result, Formatting.Indented));

                    if (result.IsOk)
                    {
                        logger.Info(message.ToString());

                        await scheduler.ScheduleJob(
                            CreateJob(
                                healthCheckMessage,
                                settings.WebhookLogCategoryId,
                                settings.SkypeConversationId,
                                settings.IsProduction,
                                settings.VerifyLogJobStartAfterSecond,
                                settings.IntervalToSendHeartbeat > 0 
                                    && countSent % settings.IntervalToSendHeartbeat == 0,
                                settings.SkypeHealthCheckHeartbeatConversationId),
                            CreateTrigger(settings.VerifyLogJobStartAfterSecond));

                        await scheduler.Start();
                        countSent++;
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
                    message.AppendLine(healthCheckMessage);
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

        private static IJobDetail CreateJob(
            string message, 
            int logCategoryId, 
            string skypeConversationId, 
            bool isProduction, 
            int periodSecond,
            bool isSendHeartbeat,
            string skypeHealthCheckHeartbeatConversationId)
            => JobBuilder.Create<VerifyLogJob>()
                .WithIdentity($"CheckLogJob_{Guid.NewGuid()}", "CheckLogJobs")
                .UsingJobData("message", message)
                .UsingJobData("logCategoryId", logCategoryId)
                .UsingJobData("skypeConversationId", skypeConversationId)
                .UsingJobData("isProduction", isProduction)
                .UsingJobData("periodSecond", periodSecond)
                .UsingJobData("isSendHeartbeat", isSendHeartbeat)
                .UsingJobData("skypeHealthCheckHeartbeatConversationId", skypeHealthCheckHeartbeatConversationId)
                .Build();

        private static ITrigger CreateTrigger(int startAfterSecond)
            => TriggerBuilder.Create()
                .WithIdentity($"CheckLogJobTrigger_{Guid.NewGuid()}", "CheckLogJobTriggers")
                .StartAt(DateTime.Now.AddSeconds(startAfterSecond))
                .Build();
    }
}