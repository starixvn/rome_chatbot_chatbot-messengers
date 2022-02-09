using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Messenger.Client.Configurations;
using Messenger.Client.Helpers;
using Messenger.Client.Models;
using Messenger.WebHooks.Site.Sample.Models;

namespace Messenger.WebHooks.Site.Sample.Controllers
{
    public class HomeController : Controller
    {
        private static int AutoIncrementId = 0;

        [HttpGet]
        public ActionResult Index()
            => View(new CommandInfo());

        [HttpPost]
        public ActionResult Index(CommandInfo commandInfo)
        {
            if (commandInfo.CommandInfoList.TryGetValue(commandInfo.Command, out var command))
            {
                var restClient = new RestClientHelper(RestClientHelper.CreateDefaultClient(commandInfo.WebhookUrl), new JwtHelper(), AppSettings.Messenger);
                command = "{" + $"{BuildCommand(commandInfo.LetstalkId)},{command}" + "}";

                try
                {
                    restClient.ExecutePost<Result>("/incoming/letstalk",
                        new Dictionary<string, object>
                        {
                            ["type"] = "message",
                            ["source"] = new
                                {
                                    type = "user",
                                    user_id = commandInfo.LetstalkId
                                },
                            ["payload"] = command
                        });

                    commandInfo.Result = new Result { Status = 1 };
                }
                catch (Exception ex)
                {
                    commandInfo.Result = new Result { Status = 2, Message = ex.Message };
                }
            }
            else
            {
                commandInfo.Result = new Result { Status = 2, Message = "Command not found." };
            }

            return View(commandInfo);
        }

        private static string BuildCommand(string letstalkId)
            => string.Format(
                "\"user_id\":\"{0}\",\"id\":{1},\"time\":{2},\"type\":0,\"hash_type\":{3},\"hash_id\":{4},\"from\":0,\"receiver\":\"{5}\",\"send_time\":{2},\"due_date\":0",
                letstalkId,
                ++AutoIncrementId,
                DateTime.Now.Ticks,
                AppSettings.Messenger.EncryptionMode,
                AppSettings.Messenger.ChatKeyVersion,
                AppSettings.Messenger.BotUserId);
    }
}