namespace Messenger.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class LetstalkClient : IMessengerClient
    {
        private const int payloadId = 100;
        private const int defaultExpiredTimeInHour = 3;
        private readonly int botId;
        private readonly IRestClientHelper restClientHelper;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly JsonSerializerSettings DefaultJsonSerializeSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public LetstalkClient(
            IMessengerSettings messengerSettings,
            IRestClientHelper restClientHelper = null,
            ICryptographyHelper cryptographyHelper = null)
        {
            this.restClientHelper = restClientHelper
                ?? new RestClientHelper(
                    RestClientHelper.CreateDefaultClient(messengerSettings.ApiEndpoint, messengerSettings.ProxyAddress),
                    new JwtHelper(),
                    messengerSettings);

            this.cryptographyHelper = cryptographyHelper ?? new CryptographyHelper();
            this.messengerSettings = messengerSettings;
            botId = this.messengerSettings.BotAccountId;
        }

        public async Task<(Contact, bool)> GetProfileAsync(string accountId)
        {
            ValidateInputParam(nameof(accountId), accountId);

#pragma warning disable S3216 // "ConfigureAwait(false)" should be used
            var result = await restClientHelper.ExecuteGetAsync<ProfileResult>($"bot/get_profile/{botId}/user/{accountId}");
#pragma warning restore S3216 // "ConfigureAwait(false)" should be used

            if (result.IsOk)
            {
                return (new Contact
                {
                    UserId = accountId,
                    AvatarUrl = result.AvatarUrl,
                    DisplayName = result.DisplayName
                }, true);
            }

            if (result.IsAccountNotExistent)
            {
                return (new Contact(), false);
            }

            throw new MessengerClientException(result.Message);
        }

        public Task<Result> PushAsync(string accountId, string message, DateTime? expiredTime = null)
            => PushAsync(new List<string> { accountId }, message, expiredTime);

        public Task<Result> PushAsync(IEnumerable<string> accountIds, string message, DateTime? expiredTime = null)
            => PushAsync(accountIds, new Message { Content = message }, expiredTime);

        public Task<Result> PushAsync(IEnumerable<string> accountIds, Message message, DateTime? expiredTime = null)
        {
            ValidateInputParam(nameof(accountIds), accountIds);
            ValidateInputParam(nameof(message), message);

            var payload = new Payload();
            var formattedMessage = JsonConvert.SerializeObject(message, DefaultJsonSerializeSetting);

            payload.HashContentKey = cryptographyHelper.Hash(formattedMessage);
            payload.HashContent = cryptographyHelper.Encrypt(formattedMessage, messengerSettings.ChatKey, payload.HashContentKey);

            payload.DueDate = message.ExpiredTime.HasValue
                    ? new DateTimeOffset(message.ExpiredTime.Value).ToUnixTimeMilliseconds()
#pragma warning disable S3358 // Ternary operators should not be nested
                    : expiredTime.HasValue
                        ? new DateTimeOffset(expiredTime.Value).ToUnixTimeMilliseconds()
#pragma warning disable S109 // Magic numbers should not be used
                        : DateTimeOffset.Now.AddHours(3).ToUnixTimeMilliseconds();
#pragma warning restore S109 // Magic numbers should not be used
#pragma warning restore S3358 // Ternary operators should not be nested

            return PushAsync(accountIds, payload);
        }

        public Task<Result> PushAsync(IEnumerable<string> accountIds, Payload payload)
        {
            ValidateInputParam(nameof(accountIds), accountIds);
            ValidateInputParam(nameof(payload), payload);

            var accountIdToLowerList = accountIds.Select(accountId => accountId.ToLowerInvariant());

            payload.UserId = messengerSettings.BotUserId;
            payload.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            payload.HashType = messengerSettings.EncryptionMode;
            payload.From = messengerSettings.BusinessAccountType;
            payload.Type = OutGoingMessageType.Text;
#pragma warning disable S109 // Magic numbers should not be used
            payload.Id = 100;
#pragma warning restore S109 // Magic numbers should not be used
            payload.Receiver = accountIdToLowerList.ElementAt(0);
            payload.HashId = messengerSettings.ChatKeyVersion;

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/push/{botId}",
                new Dictionary<string, object>
                {
                    { "receiver", accountIdToLowerList },
                    { "payload", JsonConvert.SerializeObject(payload, DefaultJsonSerializeSetting) }
                });
        }

#pragma warning disable S3994 // URI Parameters should not be strings

        public Task<Result> SetWebhookAsync(string url)
#pragma warning restore S3994 // URI Parameters should not be strings
        {
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/set_webhook/{botId}",
                new Dictionary<string, object>
                {
                    { "url", url }
                });
        }

        public Task<Result> SetWelcomeMessageAsync(string message)
        {
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecutePostAsync<Result>(
                  $"bot/set_welcome_msg/{botId}",
                  new Dictionary<string, object>
                {
                    { "message", message }
                });
        }

        public Task<Result> ChangeDisplayNameAsync(string botName)
        {
            ValidateInputParam(nameof(botName), botName);

            return restClientHelper.ExecutePutAsync<Result>(
                $"bot/change_display_name/{botId}",
                new Dictionary<string, object>
                {
                    { "display_name", botName }
                });
        }

        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateInputParam(string paramName, IEnumerable<string> paramValue)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!paramValue.Any())
            {
                throw new ArgumentOutOfRangeException($"{paramName} does not contain any item");
            }
        }

        public async Task<Result> SetAutoReplyAsync(string message, bool enabled, DateTime startTime, DateTime endTime)
        {
            ValidateInputParam(nameof(message), message);

            var result = await restClientHelper.ExecutePostAsync<Result>(
                $"bot/set_auto_reply/{botId}",
                new Dictionary<string, object>
                {
                    { "auto_reply_message", message },
                    { "enable", enabled ? 1 : 0 },
                    { "start_time",  (new DateTimeOffset(startTime)).ToUnixTimeMilliseconds() },
                    { "duration", (endTime - startTime).TotalMinutes },
                    { "duration_type", 0 }
                });

            return result;
        }

        public async Task<Result> SetAvatarAsync(string image)
        {
            ValidateInputParam(nameof(image), image);

            var result = await restClientHelper.ExecutePostAsync<Result>(
                $"bot/upload_avatar_image/{botId}",
                new Dictionary<string, object>
                {
                    { "image", image }
                });

            return result;
        }
    }
}