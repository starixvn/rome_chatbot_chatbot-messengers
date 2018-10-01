namespace Messenger.LetstalkApiMonitor.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Bot.Client;
    using Fanex.Data.Repository;
    using Messenger.LetstalkApiMonitor.DbParams.Criterias;
    using Messenger.LetstalkApiMonitor.Models;
    using Quartz;

    public class VerifyLogJob : IJob
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBotConnector botConnector;

        public VerifyLogJob() : this(null, null)
        {
        }

        public VerifyLogJob(IDynamicRepository dynamicRepository, IBotConnector botConnector)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.botConnector = botConnector ?? new BotConnector();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var message = dataMap.GetString("message");
            var logCategoryId = dataMap.GetIntValue("logCategoryId");
            var skypeConversationId = dataMap.GetString("skypeConversationId");
            var isProduction = dataMap.GetBooleanValue("isProduction");

            var logs = await GetLogs(logCategoryId, isProduction);
            var logMessage = logs.FirstOrDefault()?.FormattedMessage;

            if (string.IsNullOrEmpty(logMessage) || !logMessage.Contains(message))
            {
                botConnector.Send($"Alpha Webhook missed receiving **{message}**", skypeConversationId);
            }
        }

        private Task<IEnumerable<Log>> GetLogs(int logCategoryId, bool isProduction)
            => dynamicRepository.FetchAsync<Log>(
                new GetLogCriteria
                {
                    CategoryId = logCategoryId,
                    From = DateTime.UtcNow.AddSeconds(-65),
                    To = DateTime.UtcNow,
                    Severity = "Information",
                    Size = 1,
                    Page = 1,
                    ToGMT = 7,
                    MachineId = 0,
                    IsProduction = isProduction
                });
    }
}