namespace Chatbot.Tests.Modules.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Constants;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    public class CommandServiceTests
    {
        private readonly SettingService settingService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IFunctionHandler functionHandler;
        private readonly ILogger logger;

        private readonly CommandService commandService;

        public CommandServiceTests()
        {
            settingService = Substitute.For<SettingService>();
            messageSender = Substitute.For<IMessageSender>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            messageLogger = Substitute.For<IMessageLogger>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            functionHandler = Substitute.For<IFunctionHandler>();
            logger = Substitute.For<ILogger>();

            commandService = new CommandServiceImpl(
                settingService,
                messageSender,
                messageBuilder,
                messageLogger,
                chatbotConfigs,
                functionHandler,
                logger);
        }

        [Fact]
        public void Execute_MessengerDoesExist_ThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => commandService
                    .Execute("cm", 1, "username")
                    .GetAwaiter()
                    .GetResult());
        }

        [Fact]
        public void Execute_MessengerIsDisabled_ThrowInvalidOperationException()
        {
            var messengerType = 1;
            var messengerUsername = "mess_name";

            settingService
                .GetMessenger(messengerType, messengerUsername)
                .Returns(Task.FromResult(new MessengerDto(1, 1, "name", false, false, "code", DateTime.Now)));

            Assert.Throws<InvalidOperationException>(
                () => commandService
                    .Execute("cm", messengerType, messengerUsername)
                    .GetAwaiter()
                    .GetResult());
        }

        [Fact]
        public void Execute_MessengerIsBeingVerify_ThrowInvalidOperationException()
        {
            var messengerType = 1;
            var messengerUsername = "mess_name";

            settingService
                .GetMessenger(messengerType, messengerUsername)
                .Returns(Task.FromResult(new MessengerDto(1, 1, "name", false, true, "code", DateTime.Now)));

            Assert.Throws<InvalidOperationException>(
                () => commandService
                    .Execute("cm", messengerType, messengerUsername)
                    .GetAwaiter()
                    .GetResult());
        }

        [Fact]
        public void Execute_NotFoundCommand_ExecuteSendNotRecognizedCommandMessage()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);

            commandService
                    .Execute("cm", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageSender
                .Received()
                .SendMessage(
                userId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "NotRecognizedCommandMessage",
                100);
        }

        [Fact]
        public void Execute_FunctionIsDisabled_ExecuteSendNotRecognizedCommandMessage1()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);

            commandService
                    .Execute("cm2", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageSender
                .Received()
                .SendMessage(
                userId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "NotRecognizedCommandMessage",
                100);
        }

        [Fact]
        public void Execute_Always_LogMessage()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);

            commandService
                    .Execute("cm1", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageLogger
                .Received()
                .LogMessage(
                user.Id,
                CommonId.BotUserId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "cm1",
                0,
                1);
        }

        [Fact]
        public void Execute_Always_ExecuteHandleFunction()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);

            commandService
                    .Execute("cm1", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            functionHandler
                .Received()
                .Handle(
                    Arg.Is<FunctionParam>(function => 
                        function.User.Id == userId
                        && function.Function.Id == 1
                        && function.Messenger.Username == messenger.Username
                        && function.Others["command"] == "cm1"));
        }

        private void SetupSendCommand(int userId, Messenger messenger, User user)
        {
            settingService.GetFunctions(userId).Returns(Task.FromResult(StubFunctions()));
            settingService
                .GetMessenger(messenger.Type, messenger.Username)
                .Returns(Task.FromResult(new MessengerDto(userId, 1, "name", true, false, "code", DateTime.Now)));
            settingService.GetUser(userId).Returns(user);
            chatbotConfigs.NotRecognizedCommandFunctionId.Returns(100);
            messageBuilder.BuildNotRecognizedCommandMessage(user.Language).Returns("NotRecognizedCommandMessage");
        }

        private User StubUser(int userId)
            => new User(
                userId,
                userId + 1,
                3,
                userId + 100,
                "en-US",
                Enumerable.Empty<Messenger>(),
                Enumerable.Empty<Function>());

        private IEnumerable<Function> StubFunctions()
            => new List<Function>
            {
                new Function
                {
                    Id = 1,
                    Command = "cm1",
                    Enabled = true
                },
                new Function
                {
                    Id = 2,
                    Command = "cm2",
                    Enabled = false
                }
            };
    }
}