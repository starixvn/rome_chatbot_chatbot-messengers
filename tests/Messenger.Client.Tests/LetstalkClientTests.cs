namespace Messenger.Client.Tests
{
    using System;
    using System.Threading.Tasks;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Models;
    using NSubstitute;
    using Xunit;

    public class LetstalkClientTests
    {
        private readonly IRestClientHelper restClientHelper;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly LetstalkClient letstalkClient;

        public LetstalkClientTests()
        {
            restClientHelper = Substitute.For<IRestClientHelper>();
            cryptographyHelper = Substitute.For<ICryptographyHelper>();
            messengerSettings = Substitute.For<IMessengerSettings>();
            messengerSettings.BotAccountId.Returns(100000);

            letstalkClient = new LetstalkClient(messengerSettings, restClientHelper, cryptographyHelper);
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
    }
}