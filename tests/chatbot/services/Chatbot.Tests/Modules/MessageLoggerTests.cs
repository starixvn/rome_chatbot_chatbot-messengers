namespace Chatbot.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Configurations;
    using Chatbot.Models;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.MessageServices.Params.Db;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class MessageLoggerTests
    {
        private readonly IMessageLogger messageLogger;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly Func<DateTime> getNow;

        public MessageLoggerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            getNow = Substitute.For<Func<DateTime>>();
            messageLogger = new MessageLogger(dynamicRepository, chatbotConfigs, getNow);
        }

        [Fact]
        public void GetMessageLogs_Always_ReturnMessageLogs()
        {
            // Arrange
            var userId = 4332;
            var fromDate = new DateTime(1989, 12, 12);
            var toDate = new DateTime(1989, 12, 13);
            var messengerType = 1;
            var functionId = 1;
            var stubMessages = new List<Message> { new Message(1, 2, 3, 1, "messengername", 1, "content", new DateTime(1989, 12, 13), 1) };

            getNow.Invoke().Returns(new DateTime(1989, 12, 12));

            Expression<Predicate<GetMessageLogCriteria>> isArgFunction =
                criteria => criteria.UserId == userId
                                    && criteria.MessengerType == messengerType
                                    && criteria.FunctionId == functionId
                                    && criteria.FromDate == fromDate.Date
                                    && criteria.ToDate == toDate.Date.AddDays(1).AddTicks(-1);

            dynamicRepository.FetchAsync<Message>(Arg.Is(isArgFunction))
                .Returns(Task.FromResult(stubMessages as IEnumerable<Message>));

            // Act
            var messages = messageLogger
                .GetMessageLogs(userId, fromDate, toDate, functionId, messengerType)
                .GetAwaiter()
                .GetResult();

            // Assert
            var firstMessage = messages.First();
            Assert.Equal(1, messages.Count());
            Assert.Equal("content", firstMessage.Content);
            Assert.Equal("messengername", firstMessage.MessengerUsername);
            Assert.Equal(new DateTime(1989, 12, 13), firstMessage.SentDate);
            Assert.Equal(1, firstMessage.Id);
            Assert.Equal(2, firstMessage.Sender);
            Assert.Equal(3, firstMessage.Receiver);
        }

        [Fact]
        public void GetMessageLogs_FromDateIsLowerMinDate_ExecuteGetMessageWithMinDate()
        {
            var currentDate = new DateTime(1989, 12, 12);
            getNow.Invoke().Returns(currentDate);
            chatbotConfigs.MaxGetMessageLogDays.Returns(3);
            var fromDate = new DateTime(1989, 12, 7);

            messageLogger
                .GetMessageLogs(1, fromDate, DateTime.Now, 1, 1)
                .GetAwaiter()
                .GetResult();

            dynamicRepository.Received().FetchAsync<Message>(Arg.Is<GetMessageLogCriteria>(
                criteria => criteria.FromDate == new DateTime(1989, 12, 9)));
        }

        [Fact] 
        public void LogMessage_Always_ExecuteInsertMessageCommand()
        {
            var sender = 1;
            var receiver = 2;
            var messenger = new Messenger(1, "username", true, false);
            var message = "message content";
            var status = 1;
            var functionid = 2;

            messageLogger.LogMessage(sender, receiver, messenger, message, status, functionid);

            dynamicRepository.ExecuteAsync(Arg.Is<InsertMessageCommand>(
                command => command.Sender == sender
                            && command.Receiver == receiver
                            && command.MessengerType == messenger.Type
                            && command.MessengerUsername == messenger.Username
                            && command.Status == status
                            && command.FunctionId == functionid));
        }
    }
}
