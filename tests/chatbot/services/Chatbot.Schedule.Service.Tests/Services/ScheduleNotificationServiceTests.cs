namespace Chatbot.Schedule.Service.Tests.Services
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Quartz;
    using Xunit;

    public class ScheduleNotificationServiceTests
    {
        private readonly ScheduleNotificationService scheduleService;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobListener jobListener;
        private readonly ILogger<ScheduleNotificationService> logger;
        private readonly IConfiguration configuration;

        public ScheduleNotificationServiceTests()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            configuration = builder.Build();
            schedulerFactory = Substitute.For<ISchedulerFactory>();
            logger = Substitute.For<ILogger<ScheduleNotificationService>>();
            jobListener = Substitute.For<IJobListener>();
            scheduleService = new ScheduleNotificationService(schedulerFactory, configuration, jobListener, logger);
        }

        [Fact]
        public async Task ExecuteAsync_Always_AddJobListener()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await scheduleService.StartAsync(new CancellationToken());

            // Assert
            scheduler.ListenerManager.Received().AddJobListener(jobListener);
        }

        [Fact]
        public async Task ExecuteAsync_Always_ScheduleNotificationFunction()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await scheduleService.StartAsync(new CancellationToken());

            // Assert
            await scheduler.Received(1).ScheduleJob(
                Arg.Is<IJobDetail>(j =>
                    j.JobDataMap.GetIntValue("functionId") == 5
                    && j.Key.Equals(JobKey.Create("Personal Message", "Notification"))),
                Arg.Is<ITrigger>(t =>
                    t.Key.Equals(new TriggerKey("Personal MessageTrigger", "Notification"))));
        }

        [Fact]
        public async Task ExecuteAsync_Always_StartScheduler()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await scheduleService.StartAsync(new CancellationToken());

            // Assert
            await scheduler.Received().Start();
        }
    }
}