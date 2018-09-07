namespace Messenger.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Bot.Client;
    using Messenger.Client.Configurations;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using RestSharp;

    public class SkypeClient : IMessengerClient
    {
        private readonly IBotConnector botConnector;
        private readonly IRestClient restClient;

        public SkypeClient(
            IMessengerSettings messengerSettings,
            IBotConnector botConnector = null,
            IRestClient restClient = null)
        {
            this.botConnector = botConnector ?? new BotConnector();
            this.restClient = restClient ?? new RestClient("https://bot.nexdev.net:6969/letstalk/api");
        }

        public Task<Result> ChangeDisplayNameAsync(string botName)
        {
            throw new System.NotImplementedException();
        }

        public Task<(Contact, bool)> GetProfileAsync(string accountId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, Payload payload)
        {
            var result = botConnector.Send(payload.HashContent, payload.UserId);

            return Task.FromResult(new Result { Status = 1, Message = result });
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, string message, DateTime? expiredTime = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(string accountId, string message, DateTime? expiredTime = null)
        {
            var result = botConnector.Send(message, accountId);

            return Task.FromResult(new Result { Status = 1, Message = result });
        }

        public Task<Result> SetWebhookAsync(string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWelcomeMessageAsync(string message)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result> SetAutoReplyAsync(string message, bool enabled, DateTime startTime, DateTime endTime)
        {
            var jsonData = JsonConvert.SerializeObject(new
            {
                Message = message,
                Enable = enabled ? 1 : 0,
                StartTime = startTime.ToString(),
                EndTime = endTime.ToString()
            });
            var request = new RestRequest("/messages/SetAutoReply", Method.POST);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);

            var response = await restClient.ExecuteTaskAsync<Result>(request);

            return response.Data;
        }

        public Task<Result> SetAvatarAsync(string image)
        {
            throw new NotImplementedException();
        }
    }
}