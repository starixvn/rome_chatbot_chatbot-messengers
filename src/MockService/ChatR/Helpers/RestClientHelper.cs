namespace ChatR.Helpers
{
    using System;
    using ChatR.Configurations;
    using RestSharp;

    public interface IRestClientHelper
    {
        T Execute<T>(string resource, Method method, object body) where T : new();
    }

    public class RestClientHelper : IRestClientHelper
    {
        private readonly IRestClient restClient;

        public RestClientHelper(IRestClient restClient = null)
        {
            this.restClient = restClient ?? new RestClient(AppSettings.WebHooksUrl);
        }

        public T Execute<T>(string resource, Method method, object body) where T : new()
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var request = new RestSharp.Serializers.Newtonsoft.Json.RestRequest(resource, method);
            request.AddHeader("Content-Type", "application/json");

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            var response = restClient.Execute<T>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            throw new InvalidOperationException(response.ErrorMessage);
        }
    }
}