namespace Messenger.Client.Tests
{
    using System;
    using Fanex.Bot.Client;
    using KellermanSoftware.CompareNetObjects;
    using Messenger.Client.Configurations;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class SkypeClientTests
    {
        private readonly IBotConnector botConnector;
        private readonly IRestClient restClient;
        private readonly SkypeClient skypeClient;

        public SkypeClientTests()
        {
            botConnector = Substitute.For<IBotConnector>();
            restClient = Substitute.For<IRestClient>();
            skypeClient = new SkypeClient(Substitute.For<IMessengerSettings>(), botConnector, restClient);
        }

        [Fact]
        public void PushAsync_Always_CallsBotConnectorSendMessage()
        {
            // Arrange
            var message = "message";
            var accountId = "accountId";

            // Act
            skypeClient.PushAsync(accountId, message);

            // Assert
            botConnector.Received().Send(Arg.Is(message), Arg.Is(accountId));
        }

        [Fact]
        public void SetAutoReplyAsync_Always_CallsExecuteTaskAsync()
        {
            // Arrange
            var message = "message";
            var startTime = DateTime.Now;
            var endTime = DateTime.Now;
            var jsonData = JsonConvert.SerializeObject(new
            {
                Message = message,
                Enable = 1,
                StartTime = startTime.ToString(),
                EndTime = endTime.ToString()
            });
            var request = new RestRequest("/messages/SetAutoReply", Method.POST);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);

            var compareLogic = new CompareLogic();

            // Act
            skypeClient.SetAutoReplyAsync(message, true, startTime, endTime).ConfigureAwait(false);

            // Assert
            restClient.Received().ExecuteTaskAsync<Result>(
                Arg.Is<IRestRequest>(r => compareLogic.Compare(request, r).AreEqual));
        }
    }
}