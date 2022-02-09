namespace Messenger.WebHooks.Site.Sample.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Messenger.Client.Models;

    public class CommandInfo
    {
        // Local01
        //public IDictionary<string, string> CommandInfoList { get; }
        //    = new Dictionary<string, string>
        //    {
        //        ["/os"] = "\"hash_content\":\"jIFebHelqx5vFDkcAFEjtw==\",\"hash_content_key\":\"a507d2d570de2a029bfea57075435b1170835e52\"",
        //        ["/wl"] = "\"hash_content\":\"Dvwof3UlhVLVfi9OIBFY1w==\",\"hash_content_key\":\"e1d747f6eafec5002bcc382408613f459d3ed867\"",
        //        ["/bal"] = "\"hash_content\":\"j4VnkVS38pnhLnzU2/e2MQ==\",\"hash_content_key\":\"dd1f37124620045ba025497edd328df41287ad7b\"",
        //        ["/not-found-command"] = "\"hash_content\":\"hZIijSW9gRE6LYKi3vMUtQ==\",\"hash_content_key\":\"f8fea8e0c458f535e25205b988936520736e8bc2\"",
        //    };


        // Local02
        //public IDictionary<string, string> CommandInfoList { get; }
        //    = new Dictionary<string, string>
        //    {
        //        ["/os"] = "\"hash_content\":\"YpNIwhVYn3LszOrhMZDweg==\",\"hash_content_key\":\"bdf5ae859a340ad0dd2d256fd921dbbb11c39320\"",
        //        ["/wl"] = "\"hash_content\":\"s2YNW8x7sokiiE0y63k51w==\",\"hash_content_key\":\"e9ed25360b089396fa0a8e84255c35787e7e9069\"",
        //        ["/bal"] = "\"hash_content\":\"91mA3oz6cYHYuPb/kpSkkQ==\",\"hash_content_key\":\"31037af04b2f9cf5b7cd0d763f703d988ec67da3\"",
        //        ["/not-found-command"] = "\"hash_content\":\"j+Mppteqgn0Le9Bz9yzTbQ==\",\"hash_content_key\":\"3db82dd93d7d2f75d21556fca317272ae29f20d7\"",
        //    };

        // Local03
        public IDictionary<string, string> CommandInfoList { get; }
            = new Dictionary<string, string>
            {
                ["/os"] = "\"hash_content\":\"eAZpkMWKWk77k1wSoTpIeQ==\",\"hash_content_key\":\"c9f9bd3373cbee049bea80621a2d23a61cf4bbde\"",
                ["/wl"] = "\"hash_content\":\"qLr5+MtL+rVy5AK7yEuJGg==\",\"hash_content_key\":\"a0daad64c8d4df7309069832b1a31c98a8924874\"",
                ["/bal"] = "\"hash_content\":\"j2wVsSfXods9Ot9W9lJ6cg==\",\"hash_content_key\":\"5715eb5536c4103d80f00756c5f4af50390fcd4b\"",
                ["/not-found-command"] = "\"hash_content\":\"phXhvsdwdcmw5mCIhfRzFw==\",\"hash_content_key\":\"1d3f07a3133009d989493ab3d5b8e1ba8c0a1f2f\"",
            };

        public IEnumerable<string> WebhookUrls { get; }
            = ConfigurationManager.AppSettings["WebhookUrls"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        public string LetstalkId { get; } = "t233663";

        public string WebhookUrl { get; set; }

        public string Command { get; set; }

        public Result Result { get; set; }
    }
}
