namespace Messenger.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KellermanSoftware.CompareNetObjects;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class LetstalkClientTests
    {
        private readonly IRestClientHelper restClientHelper;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly LetstalkClient letstalkClient;
        private readonly CompareLogic compareLogic;

        public LetstalkClientTests()
        {
            restClientHelper = Substitute.For<IRestClientHelper>();
            cryptographyHelper = Substitute.For<ICryptographyHelper>();
            messengerSettings = Substitute.For<IMessengerSettings>();
            messengerSettings.BotAccountId.Returns(100000);
            messengerSettings.ChatKey.Returns("ChatKey");

            letstalkClient = new LetstalkClient(messengerSettings, restClientHelper, cryptographyHelper);
            compareLogic = new CompareLogic();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetProfileAsync_AccountIdIsNullOrWhiteSpace_ThrowsArgumentNullException(string accountId)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.GetProfileAsync(accountId));
        }

        [Fact]
        public void GetProfileAsync_AccountIdIsValid_CallsRestClientHelperExecuteGetAsync()
        {
            // Arrange
            var accountId = "AccountId";
            var profileResult = new ProfileResult { Status = 1 };
            restClientHelper.ExecuteGetAsync<ProfileResult>(Arg.Any<string>())
                .Returns(Task.FromResult(profileResult));

            // Act
            letstalkClient.GetProfileAsync(accountId).ConfigureAwait(false);

            // Assert
            restClientHelper.Received().ExecuteGetAsync<ProfileResult>(
                Arg.Is($"bot/get_profile/{messengerSettings.BotAccountId}/user/{accountId}"));
        }

        [Fact]
        public void GetProfileAsync_ResultIsOk_ReturnsCorrectResult()
        {
            // Arrange
            var accountId = "AccountId";
            var profileResult = new ProfileResult
            {
                Status = 1,
                AvatarUrl = "AvatarUrl",
                DisplayName = "DisplayName"
            };
            restClientHelper.ExecuteGetAsync<ProfileResult>(Arg.Any<string>())
                .Returns(Task.FromResult(profileResult));

            var expectedResult = (new Contact
            {
                UserId = accountId,
                AvatarUrl = profileResult.AvatarUrl,
                DisplayName = profileResult.DisplayName
            }, true);

            // Act
            var actualResult = letstalkClient.GetProfileAsync(accountId)
                .GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void GetProfileAsync_AccountIsNotExistent_ReturnsCorrectResult()
        {
            // Arrange
            var accountId = "AccountId";
            var profileResult = new ProfileResult { Status = 21, };
            restClientHelper.ExecuteGetAsync<ProfileResult>(Arg.Any<string>())
                .Returns(Task.FromResult(profileResult));

            var expectedResult = (new Contact(), false);

            // Act
            var actualResult = letstalkClient.GetProfileAsync(accountId)
                .GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void GetProfileAsync_UnhandleResultStatus_ThrowsMessengerClientException()
        {
            // Arrange
            var accountId = "AccountId";
            var profileResult = new ProfileResult { Status = 2, };
            restClientHelper.ExecuteGetAsync<ProfileResult>(Arg.Any<string>())
                .Returns(Task.FromResult(profileResult));

            // Act & Assert
            Assert.ThrowsAsync<MessengerClientException>(() => letstalkClient.GetProfileAsync(accountId));
        }

        [Fact]
        public void PushAsync_AccountIdsIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.PushAsync((IEnumerable<string>)null, string.Empty));
        }

        [Fact]
        public void PushAsync_AccountIdsIsEmptyList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => letstalkClient.PushAsync(Enumerable.Empty<string>(), string.Empty));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void PushAsync_MessageIsNullOrWhiteSpace_ThrowsArgumentNullException(string message)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.PushAsync(new List<string> { "account" }, message));
        }

        [Fact]
        public void PushAsync_ValidParams_CallsCryptographyHelperHashMessage()
        {
            // Arrange
            var message = "message";
            var formattedMessage = JsonConvert.SerializeObject(new { msg = message });

            // Act
            letstalkClient.PushAsync(new List<string> { "account" }, message, DateTime.Now);

            //Assert
            cryptographyHelper.Received().Hash(Arg.Is(formattedMessage));
        }

        [Fact]
        public void PushAsync_ValidParams_CallsCryptographyHelperEncrypt()
        {
            // Arrange
            var message = "message";
            var formattedMessage = JsonConvert.SerializeObject(new { msg = message });

            var hashContentKey = "HashContentKey";
            cryptographyHelper.Hash(Arg.Any<string>()).Returns(hashContentKey);

            // Act
            letstalkClient.PushAsync(new List<string> { "account" }, message);

            //Assert
            cryptographyHelper.Received().Encrypt(
                Arg.Is(formattedMessage),
                Arg.Is(messengerSettings.ChatKey),
                Arg.Is(hashContentKey));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetWebhookAsync_UrlIsNullOrWhiteSpace_ThrowsArgumentNullException(string url)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.SetWebhookAsync(url));
        }

        [Fact]
        public void SetWebhookAsync_ValidUrl_CallsExecutePostAsync()
        {
            // Arrange
            var url = "url";

            // Act
            letstalkClient.SetWebhookAsync(url);

            // Assert
            restClientHelper.Received().ExecutePostAsync<Result>(
                Arg.Is($"bot/set_webhook/{messengerSettings.BotAccountId}"),
                Arg.Is<Dictionary<string, object>>(d => d["url"].Equals(url)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetWelcomeMessageAsync_MessageIsNullOrWhiteSpace_ThrowsArgumentNullException(string message)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.SetWelcomeMessageAsync(message));
        }

        [Fact]
        public void SetWelcomeMessageAsync_ValidMessage_CallsExecutePostAsync()
        {
            // Arrange
            var message = "message";

            // Act
            letstalkClient.SetWelcomeMessageAsync(message);

            // Assert
            restClientHelper.Received().ExecutePostAsync<Result>(
                Arg.Is($"bot/set_welcome_msg/{messengerSettings.BotAccountId}"),
                Arg.Is<Dictionary<string, object>>(d => d["message"].Equals(message)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeDisplayNameAsync_BotNameIsNullOrWhiteSpace_ThrowsArgumentNullException(string botName)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.ChangeDisplayNameAsync(botName));
        }

        [Fact]
        public void ChangeDisplayNameAsync_ValidBotName_CallsExecutePutAsync()
        {
            // Arrange
            var botName = "BotName";

            // Act
            letstalkClient.ChangeDisplayNameAsync(botName);

            // Assert
            restClientHelper.Received().ExecutePutAsync<Result>(
                Arg.Is($"bot/change_display_name/{messengerSettings.BotAccountId}"),
                Arg.Is<Dictionary<string, object>>(d => d["display_name"].Equals(botName)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetAutoReplyAsync_MessageIsNullOrWhiteSpace_ThrowsArgumentNullException(string message)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.SetAutoReplyAsync(message, true, DateTime.Now, DateTime.Now));
        }

        [Fact]
        public void SetAutoReplyAsync_ValidMessage_CallsExecutePostAsync()
        {
            // Arrange
            var message = "message";
            var enable = true;
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddHours(1);

            // Act
            letstalkClient.SetAutoReplyAsync(message, enable, startTime, endTime).ConfigureAwait(false);

            // Assert
            restClientHelper.Received().ExecutePostAsync<Result>(
                Arg.Is($"bot/set_auto_reply/{messengerSettings.BotAccountId}"),
                Arg.Is<Dictionary<string, object>>(
                    d => d["auto_reply_message"].Equals(message)
                        && d["enable"].Equals(1)
                        && d["start_time"].Equals(new DateTimeOffset(startTime).ToUnixTimeMilliseconds())
                        && d["duration"].Equals((endTime - startTime).TotalMinutes)
                        && d["duration_type"].Equals(0)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetAvatarAsync_ImageIsNullOrWhiteSpace_ThrowsArgumentNullException(string image)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => letstalkClient.SetAvatarAsync(image));
        }

        [Fact]
        public void SetAvatarAsync_ValidImage_CallsExecutePostAsync()
        {
            // Arrange
            var image = "Image";

            // Act
            letstalkClient.SetAvatarAsync(image).ConfigureAwait(false);

            // Assert
            restClientHelper.Received().ExecutePostAsync<Result>(
                Arg.Is($"bot/upload_avatar_image/{messengerSettings.BotAccountId}"),
                Arg.Is<Dictionary<string, object>>(d => d["image"].Equals(image)));
        }
    }
}