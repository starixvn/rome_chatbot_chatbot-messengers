namespace Messenger.WebHooks.Sample
{
    using System.Collections.Generic;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Models;

    internal static class Program
    {
        private static void Main()
        {
            var botId = AppSettings.Messenger.BotAccountId;
            var restClient = new RestClientHelper(RestClientHelper.CreateDefaultClient(AppSettings.Messenger.ApiEndpoint), new JwtHelper(), AppSettings.Messenger);

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            var wlCommnand = "{\"user_id\":\"starix_test96\",\"id\":1271,\"time\":1534842405686,\"type\":0,\"hash_type\":5,\"hash_content\":\"f1m+JUQnHlNjtdF0iXgcLw==\",\"hash_content_key\":\"591febf4006d3e52f1a2847b8fd1938942e7fb4f\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842405719,\"duedate\":1537434405719}";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            var balCommnand = "{\"user_id\":\"starix_test99\",\"id\":1279,\"time\":1534842731491,\"type\":0,\"hash_type\":5,\"hash_content\":\"Axm3Iu8xFrPwrs19J0K97Q==\",\"hash_content_key\":\"456d0b7e0328e8fb9fdf6510a035599e26923aec\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842731523,\"duedate\":1537434731523}";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            var osCommnand = "{\"user_id\":\"starix_test99\",\"id\":1277,\"time\":1534842727531,\"type\":0,\"hash_type\":5,\"hash_content\":\"W2SWS8vQBUpw9zFeaTzvNQ==\",\"hash_content_key\":\"e1da28443b9d65ae59426c49610292566465b3e6\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842727568,\"duedate\":1537434727568}";

            restClient.ExecutePost<Result>("/incoming/letstalk",
                new Dictionary<string, object>
                {
                    { "type", "message" },
                    { "source", new
                            {
                                type = "user",
                                user_id = "starix_test96"
                            }
                    },
                    { "payload", osCommnand }
                });
        }

        //            {
        //  "type": "message",
        //  "timestamp": "1534842406450",
        //  "source": {
        //    "type": "user",
        //    "user_id": "starix_test99"
        //  },
        //  "payload": "{\"user_id\":\"starix_test99\",\"id\":1271,\"time\":1534842405686,\"type\":0,\"hash_type\":5,\"hash_content\":\"f1m+JUQnHlNjtdF0iXgcLw==\",\"hash_content_key\":\"591febf4006d3e52f1a2847b8fd1938942e7fb4f\",\"hash_id\":3,\"from\":0,\"receiver\":\"@nexdev.bot\",\"send_time\":1534842405719,\"duedate\":1537434405719}",
        //  "iat": 1534842406,
        //  "exp": 1534847406
        //}

        //var restClient = CreateRestClient();

        //PushMessageEvent(restClient);
        //PushAddContactEvent(restClient);
        //PushDeleteContactEvent(restClient);
        //PushAskKeyEvent(restClient);
        //PushHealthCheckEvent(restClient);
    }

    //private static void PushMessageEvent(IRestClientHelper restClient)
    //    =>

    //    private static void PushAddContactEvent(IRestClientHelper restClient)
    //        => Execute(
    //            restClient,
    //            "/incoming/letstalk",
    //            Method.POST,
    //            new
    //            {
    //                type = "add_contact",
    //                timestamp = 1527151205790,
    //                source = new
    //                {
    //                    type = "user",
    //                    user_id = "jimmy.chen"
    //                }
    //            });

    //    private static void PushDeleteContactEvent(IRestClient restClient)
    //        => Execute(
    //            restClient,
    //            "/incoming/letstalk",
    //            Method.POST,
    //            new
    //            {
    //                type = "unsubscribe",
    //                timestamp = 1527151205790,
    //                source = new
    //                {
    //                    type = "user",
    //                    user_id = "jimmy.chen"
    //                }
    //            });

    //    private static void PushAskKeyEvent(IRestClientHelper restClient)
    //        => Execute(
    //            restClient,
    //            "/incoming/letstalk",
    //            Method.POST,
    //            new
    //            {
    //                type = "ask_key",
    //                timestamp = 1527151205790,
    //                source = new
    //                {
    //                    type = "user",
    //                    user_id = "jimmy.chen"
    //                }
    //            });

    //    private static void PushHealthCheckEvent(IRestClientHelper restClient)
    //        => Execute(
    //            restClient,
    //            "/incoming/letstalk",
    //            Method.POST,
    //            new
    //            {
    //                type = "health_check",
    //                timestamp = 1527151205790
    //            });
    //}
}