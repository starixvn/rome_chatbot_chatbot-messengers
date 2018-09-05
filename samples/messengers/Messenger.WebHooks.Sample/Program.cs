namespace Messenger.WebHooks.Sample
{
    using System;
    using Newtonsoft.Json;
    using RestSharp;

    internal static class Program
    {
        private static void Main()
        {
            var restClient = CreateRestClient();

            PushMessageEvent(restClient);
            PushAddContactEvent(restClient);
            PushDeleteContactEvent(restClient);
            PushAskKeyEvent(restClient);
            PushHealthCheckEvent(restClient);
        }

        private static void PushMessageEvent(IRestClient restClient)
            => Execute(
                restClient,
                "/incoming/letstalk",
                Method.POST,
                new
                {
                    type = "message",
                    timestamp = 1527151205790,
                    source = new
                    {
                        type = "user",
                        user_id = "jimmy.chen"
                    },
                    payload = "encrypted_payload"
                });

        private static void PushAddContactEvent(IRestClient restClient)
            => Execute(
                restClient,
                "/incoming/letstalk",
                Method.POST,
                new
                {
                    type = "add_contact",
                    timestamp = 1527151205790,
                    source = new
                    {
                        type = "user",
                        user_id = "jimmy.chen"
                    }
                });

        private static void PushDeleteContactEvent(IRestClient restClient)
            => Execute(
                restClient,
                "/incoming/letstalk",
                Method.POST,
                new
                {
                    type = "unsubscribe",
                    timestamp = 1527151205790,
                    source = new
                    {
                        type = "user",
                        user_id = "jimmy.chen"
                    }
                });

        private static void PushAskKeyEvent(IRestClient restClient)
            => Execute(
                restClient,
                "/incoming/letstalk",
                Method.POST,
                new
                {
                    type = "ask_key",
                    timestamp = 1527151205790,
                    source = new
                    {
                        type = "user",
                        user_id = "jimmy.chen"
                    }
                });

        private static void PushHealthCheckEvent(IRestClient restClient)
            => Execute(
                restClient,
                "/incoming/letstalk",
                Method.POST,
                new
                {
                    type = "health_check",
                    timestamp = 1527151205790
                });

        private static IRestClient CreateRestClient()
            => new RestClient("http://localhost:57648/api/webhooks/");

        private static void Execute(IRestClient restClient, string endPoint, Method method, object data)
        {
            //incoming/letstalk/
            var request = new RestSharp.Serializers.Newtonsoft.Json.RestRequest(endPoint, method);
            request.AddHeader("Content-Type", "application/json");

            if (data != null)
            {
                request.AddJsonBody(data);
            }

            var response = restClient.Execute<object>(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine($"Pushed: {JsonConvert.SerializeObject(response.Data ?? data)}");
                return;
            }

            Console.WriteLine($"Error: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        }
    }
}