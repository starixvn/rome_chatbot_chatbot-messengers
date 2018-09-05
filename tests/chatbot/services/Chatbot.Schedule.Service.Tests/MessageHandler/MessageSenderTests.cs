namespace Chatbot.Schedule.Service.Tests.MessageHandler
{
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.MessageHandler;
    using NSubstitute;
    using Quartz;
    using Xunit;

    public class MessageSenderTests
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobListener jobListener;
        private readonly IMessageSender messageSender;

        public MessageSenderTests()
        {
            schedulerFactory = Substitute.For<ISchedulerFactory>();
            jobListener = Substitute.For<IJobListener>();
            messageSender = new MessageSender(schedulerFactory, jobListener);
        }

        [Fact]
        public async Task SendMessageByFunction_Always_AddJobListener()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await messageSender.SendMessageByFunction(1, 1);

            // Assert
            scheduler.ListenerManager.Received().AddJobListener(jobListener);
        }

        [Fact]
        public async Task SendMessageByFunction_Always_CallScheduleJobWithCorrectJobAndTrigger()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await messageSender.SendMessageByFunction(1, 1);

            // Assert
            await scheduler.Received(1).ScheduleJob(
              Arg.Is<IJobDetail>(j =>
                  j.JobDataMap.GetIntValue("functionId") == 1
                  && j.JobDataMap.GetIntValue("userId") == 1),
              Arg.Any<ITrigger>());
        }
    }
}