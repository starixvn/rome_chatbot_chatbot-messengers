using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Messenger.Client.Configurations;
using Messenger.Client.Helpers;
using Messenger.Client.Models;
using Messenger.WebHooks.Site.Sample.Models;

namespace Messenger.WebHooks.Site.Sample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var commandInfo = new CommandInfo();
            return View(commandInfo);
        }


        [HttpPost]
        public ActionResult Index(CommandInfo commandInfo)
        {
            var restClient = new RestClientHelper(RestClientHelper.CreateDefaultClient(commandInfo.WebhookUrl), new JwtHelper(), AppSettings.Messenger);

            var command = commandInfo.CommandInfoList.Keys.Contains(commandInfo.Command) 
                ? commandInfo.CommandInfoList[commandInfo.Command].Replace("{0}", commandInfo.LetstalkId)
                : commandInfo.Command;

            try
            {
                restClient.ExecutePost<Result>("/incoming/letstalk",
                    new Dictionary<string, object>
                    {
                    { "type", "message" },
                    { "source", new
                            {
                                type = "user",
                                user_id = commandInfo.LetstalkId
                            }
                    },
                    { "payload", command }
                    });

                commandInfo.Result = new Result { Status = 1 };
            }
            catch(Exception ex)
            {
                commandInfo.Result = new Result { Status = 2, Message = ex.Message };
            }

            return View(commandInfo);
        }
    }
}