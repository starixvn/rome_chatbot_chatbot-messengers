namespace Chatbot.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageBuilders.Templates;
    using NSubstitute;
    using Xunit;

    public class MessageBuilderTests
    {
        private readonly IDictionary<int, IMessengerFormatter> messengerFormatters;
        private readonly IDictionary<string, CultureInfo> cultureInfoList;
        private readonly IDictionary<int, IMessageTemplate> messageTemplates;
        private readonly IMessengerFormatter messengerFormatter;
        private readonly IMessageTemplate messageTemplate;
        private readonly MessageBuilder messageBuilder;

        public MessageBuilderTests()
        {
            messageTemplate = Substitute.For<IMessageTemplate>();
            messengerFormatter = Substitute.For<IMessengerFormatter>();
            messengerFormatters = new Dictionary<int, IMessengerFormatter>
            {
                { 1, messengerFormatter }
            };
            messageTemplates = new Dictionary<int, IMessageTemplate>
            {
                { 1, messageTemplate }
            };
            cultureInfoList = new Dictionary<string, CultureInfo>();
            messengerFormatter.NewLine().Returns("_newline_");

            messageBuilder = new MessageBuilder(cultureInfoList, messageTemplates, messengerFormatters);
        }

        [Fact]
        public void BuildVerificationCodeMessage_Always_ReturnMessage()
        {
            var language = "en-US";
            var messengerType = 1;
            var verificationCode = "422134";
            var expiredVerificationCodeInMinutes = 10;

            var verificationCodeMessage = messageBuilder.BuildVerificationCodeMessage(
                language, 
                messengerType, 
                verificationCode, 
                expiredVerificationCodeInMinutes);

            Assert.Equal(
                "You have requested to set up Chatbot. "
                + "Your verification code is 422134._newline_The code will expire in 10 minutes.", 
                verificationCodeMessage);
        }

        [Fact]
        public void BuildNotRecognizedCommandMessage_Always_ReturnMessage()
        {
            var language = "en-US";

            var message = messageBuilder.BuildNotRecognizedCommandMessage(language);

            Assert.Equal(
                "Your command is not recognized.",
                message);
        }

        [Fact]
        public void BuildWelcomeMessage_Always_ReturnMessage()
        {
            var language = "en-US";

            var message = messageBuilder.BuildWelcomeMessage(language, 1);

            Assert.Equal(
                "Your Letstalk ID has been verified, Chatbot is fully functioning._newline_You can turn off any feature via the Chatbot configuration page on your agency site.",
                message);
        }

        [Fact]
        public void BuildAccountClosedMessage_Always_ReturnMessage()
        {
            var language = "en-US";

            var message = messageBuilder.BuildAccountClosedMessage(language);

            Assert.Equal(
                "Your account is closed.",
                message);
        }

        [Fact]
        public void BuildUnderMaintenanceMessage_Always_ReturnMessage()
        {
            var message = messageBuilder.BuildUnderMaintenanceMessage("2:00", "3:00");

            Assert.Equal(
                "Sorry, the system is under maintenance from 2:00 to 3:00 GMT+8. Please come back later.",
                message);
        }

        [Fact]
        public void BuildCommandResultMessage_Always_ReturnMessage()
        {
            var functionId = 1;
            var data = new Dictionary<string, object>
            {
                { "key", "value" }
            };
            var messengerType = 1;
            var language = "en-US";

            messageTemplate
                .ToString(functionId, data, Arg.Any<CultureInfo>(), messengerFormatter)
                .Returns("Builded Message");

            var message = messageBuilder.BuildCommandResultMessage(language, functionId, messengerType, data);

            Assert.Equal("Builded Message", message);
        }


    }
}
