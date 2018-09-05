namespace Chatbot.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;
    using Messenger.Client;
    using Chatbot.Constants;

    public class MessageSenderTests
    {
        private readonly MessengerFactory messengerFactory;
        private readonly IMessageLogger messageLogger;
        private readonly ILogger logger;
        private readonly MessageSender messageSender;
        private readonly IMessengerClient messengerClient;

        public MessageSenderTests()
        {
            messengerFactory = Substitute.For<MessengerFactory>();
            messageLogger = Substitute.For<IMessageLogger>();
            logger = Substitute.For<ILogger>();
            messengerClient = Substitute.For<IMessengerClient>();

            messageSender = new MessageSender(messengerFactory, logger, messageLogger);
        }

        [Fact]
        public void SendMessage_Always_CreateMessengerClient()
        {
            var userId = 2;
            var messenger = new Chatbot.Models.Messenger(1, "messengername");
            var message = "message content";
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new Messenger.Client.Models.Result());
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            messageSender.SendMessage(userId, messenger, message).GetAwaiter().GetResult();

            messengerFactory.Received().CreateMessengerClient(messenger.Type);
        }

        [Fact]
        public void SendMessage_PushMessageFailed_LogError()
        {
            var userId = 2;
            var messenger = new Chatbot.Models.Messenger(1, "messengername");
            var message = "message content";
            var failedResult = new Messenger.Client.Models.Result { Status = 123, Message = "Test Message" };
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(failedResult);
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            messageSender.SendMessage(userId, messenger, message).GetAwaiter().GetResult();

            logger.Received().Error("Deliver message failed: message content\r\nCode: 123, Message: Test Message");
        }

        [Fact]
        public void SendMessage_SettingLogToDatabase_ExecuteLogMessage()
        {
            var userId = 2;
            var messenger = new Chatbot.Models.Messenger(1, "messengername");
            var message = "message content";
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new Messenger.Client.Models.Result { Status = 1 });
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            messageSender.SendMessage(userId, messenger, message, 3, true).GetAwaiter().GetResult();

            messageLogger.Received().LogMessage(CommonId.BotUserId, userId, messenger, message, 1, 3);
        }

        [Fact]
        public void SendMessage_Always_ReturnSucccesResult()
        {
            var userId = 2;
            var messenger = new Models.Messenger(1, "messengername");
            var message = "message content";
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new Messenger.Client.Models.Result { Status = 1 });
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            var result = messageSender.SendMessage(userId, messenger, message, 3, true).GetAwaiter().GetResult();

            Assert.True(result.IsOk);
        }
    }
}
