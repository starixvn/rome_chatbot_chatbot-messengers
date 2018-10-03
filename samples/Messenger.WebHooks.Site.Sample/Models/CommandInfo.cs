namespace Messenger.WebHooks.Site.Sample.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Messenger.Client.Models;

    public class CommandInfo
    {
        private readonly static Dictionary<string, string> commandInfoList
            = new Dictionary<string, string>
            {
                { "/os", "{\"user_id\":\"{0}\",\"id\":1277,\"time\":1534842727531,\"type\":0,\"hash_type\":5,\"hash_content\":\"b46Fqi5ox0eJ1uXTaYOmcA==\",\"hash_content_key\":\"e1da28443b9d65ae59426c49610292566465b3e6\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842727568,\"duedate\":1537434727568}" },
                { "/wl", "{\"user_id\":\"{0}\",\"id\":1271,\"time\":1534842405686,\"type\":0,\"hash_type\":5,\"hash_content\":\"JxQwaJhHJH88RU3GsjvAqw==\",\"hash_content_key\":\"591febf4006d3e52f1a2847b8fd1938942e7fb4f\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842405719,\"duedate\":1537434405719}" },
                { "/bal", "{\"user_id\":\"{0}\",\"id\":1279,\"time\":1534842731491,\"type\":0,\"hash_type\":5,\"hash_content\":\"VqPtaqforPlao2KYHMBvqw==\",\"hash_content_key\":\"456d0b7e0328e8fb9fdf6510a035599e26923aec\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842731523,\"duedate\":1537434731523}" },
                { "/not-found-command", "{\"user_id\":\"{0}\",\"id\":1293,\"time\":1534842920137,\"type\":0,\"hash_type\":5,\"hash_content\":\"nu+KqY8aF6sYQ7LYHDgk833xr01E0mHBk1eaBqgGW5E=\",\"hash_content_key\":\"8f979568c439244e71edf9200c7241c2c4cc5719\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842920168,\"duedate\":1537434920168}" }
            };

        private readonly static IEnumerable<string> webhookUrls
            = ConfigurationManager.AppSettings["WebhookUrls"].Split(',').AsEnumerable();

        public string LetstalkId { get; set; } = "starix_test96";

        public string WebhookUrl { get; set; }

        public string Command { get; set; }

        public Result Result { get; set; }

        public Dictionary<string, string> CommandInfoList => commandInfoList;

        public IEnumerable<string> WebhookUrls => webhookUrls;
    }
}