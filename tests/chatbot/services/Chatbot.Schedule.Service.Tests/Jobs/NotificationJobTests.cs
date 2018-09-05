namespace Chatbot.Schedule.Service.Tests.Jobs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.Jobs;
    using Chatbot.Schedule.Service.MessageHandler;
    using NSubstitute;
    using Quartz;
    using Xunit;

    public class NotificationJobTests
    {
        private readonly IMessageSender messageSender;
        private readonly SettingService settingService;
        private readonly IJob notificationJob;

        public NotificationJobTests()
        {
            messageSender = Substitute.For<IMessageSender>();
            settingService = Substitute.For<SettingService>();
            notificationJob = new NotificationJob(messageSender, settingService);
        }

        [Fact]
        public async Task Execute_Always_SendMessageForEachUser()
        {
            // Arrange
            var context = Substitute.For<IJobExecutionContext>();
            IDictionary<string, object> keyValuePairs = new Dictionary<string, object> { { "functionId", 4 } };
            context.JobDetail.JobDataMap.Returns(new JobDataMap(keyValuePairs));
            settingService.GetUsersByFunction(4, 0).Returns(
                new List<User> {
                    new User { CustId = 1, Id = 2 },
                    new User { CustId = 2, Id = 3 }
                });

            // Act
            await notificationJob.Execute(context);

            // Assert
            await messageSender.Received().SendMessageByFunction(2, 4);
            await messageSender.Received().SendMessageByFunction(3, 4);
            await messageSender.Received(2).SendMessageByFunction(Arg.Any<int>(), Arg.Any<int>());
        }
    }
}