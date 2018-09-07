namespace Messenger.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public interface IMessengerClient
    {
        Task<Result> PushAsync(IEnumerable<string> accountIds, Payload payload);

        Task<Result> PushAsync(IEnumerable<string> accountIds, string message, DateTime? expiredTime = null);

        Task<Result> PushAsync(string accountId, string message, DateTime? expiredTime = null);

        Task<(Contact, bool)> GetProfileAsync(string accountId);

        Task<Result> SetWebhookAsync(string url);

        Task<Result> SetAvatarAsync(string image);

        Task<Result> SetWelcomeMessageAsync(string message);

        Task<Result> SetAutoReplyAsync(string message, bool enabled, DateTime startTime, DateTime endTime);

        Task<Result> ChangeDisplayNameAsync(string botName);
    }
}