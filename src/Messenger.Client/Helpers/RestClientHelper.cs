namespace Messenger.Client.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Messenger.Client.Configurations;
    using RestSharp;

    public interface IRestClientHelper
    {
        T ExecuteGet<T>(string endPoint, IDictionary<string, object> data = null) where T : new();

        T ExecutePost<T>(string endPoint, IDictionary<string, object> data) where T : new();

        T ExecutePut<T>(string endPoint, IDictionary<string, object> data) where T : new();

        T ExecuteDelete<T>(string endPoint, IDictionary<string, object> data) where T : new();

        Task<T> ExecuteGetAsync<T>(string endPoint, IDictionary<string, object> data = null) where T : new();

        Task<T> ExecutePostAsync<T>(string endPoint, IDictionary<string, object> data) where T : new();

        Task<T> ExecutePutAsync<T>(string endPoint, IDictionary<string, object> data) where T : new();

        Task<T> ExecuteDeleteAsync<T>(string endPoint, IDictionary<string, object> data) where T : new();
    }

    public class RestClientHelper : IRestClientHelper
    {
        private const string ContentType = "text/plain";
        private const int ExpiredTokenPeriodInMiliseconds = 5000;
        private readonly IRestClient restClient;
        private readonly IJwtHelper jwtHelper;
        private readonly IMessengerSettings messengerSettings;

        public RestClientHelper(
            IRestClient restClient,
            IJwtHelper jwtHelper,
            IMessengerSettings messengerSettings)
        {
            this.restClient = restClient;
            this.jwtHelper = jwtHelper;
            this.messengerSettings = messengerSettings;
        }

        public T ExecuteGet<T>(string endPoint, IDictionary<string, object> data = null)
            where T : new()
            => Execute<T>(endPoint, Method.GET, data);

        public T ExecutePost<T>(string endPoint, IDictionary<string, object> data)
            where T : new()
            => Execute<T>(endPoint, Method.POST, data);

        public T ExecutePut<T>(string endPoint, IDictionary<string, object> data)
            where T : new()
            => Execute<T>(endPoint, Method.PUT, data);

        public T ExecuteDelete<T>(string endPoint, IDictionary<string, object> data)
            where T : new()
            => Execute<T>(endPoint, Method.DELETE, data);

        private T Execute<T>(string endPoint, Method method, IDictionary<string, object> data)
            where T : new()
        {
            ValidateEndPoint(endPoint);
            var request = CreateRequest(endPoint, method);

            if (data != null)
            {
                var encodedData = Encode(data);
                request.AddParameter(ContentType, encodedData, ParameterType.RequestBody);
            }

            var response = restClient.Execute<T>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            throw new InvalidOperationException(response.ErrorMessage);
        }

        private RestRequest CreateRequest(string endPoint, Method method)
        {
            var request = new RestRequest(endPoint, method);

            request.AddHeader("Content-Type", ContentType);
            request.AddHeader("Authorization", $"Bearer {EncodeBase64(messengerSettings.ApiKey)}");

            return request;
        }

        private async Task<T> ExecuteSync<T>(string endPoint, Method method, IDictionary<string, object> data)
            where T : new()
        {
            ValidateEndPoint(endPoint);

            var request = CreateRequest(endPoint, method);

            if (data != null)
            {
                var encodedData = Encode(data);
                request.AddParameter(ContentType, encodedData, ParameterType.RequestBody);
            }

            var response = await restClient.ExecuteTaskAsync<T>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            try
            {
                var errorResponseJson = SimpleJson.SimpleJson.SerializeObject(response);

                throw new InvalidOperationException(errorResponseJson);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    string.Join(
                    "\r\n",
                    $"Content: {response.Content}",
                    $"Status Code: {response.StatusCode}",
                    $"Response Status: {response.ResponseStatus}",
                    $"Error Message: {response.ErrorMessage}"),
                    exception);
            }
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        public Task<T> ExecuteGetAsync<T>(string endPoint, IDictionary<string, object> data = null) where T : new()
            => ExecuteSync<T>(endPoint, Method.GET, data);

        public Task<T> ExecutePostAsync<T>(string endPoint, IDictionary<string, object> data) where T : new()
            => ExecuteSync<T>(endPoint, Method.POST, data);

        public Task<T> ExecutePutAsync<T>(string endPoint, IDictionary<string, object> data) where T : new()
            => ExecuteSync<T>(endPoint, Method.PUT, data);

        public Task<T> ExecuteDeleteAsync<T>(string endPoint, IDictionary<string, object> data) where T : new()
            => ExecuteSync<T>(endPoint, Method.DELETE, data);

        public static IRestClient CreateDefaultClient(string apiEndpoint, string proxyAddress)
            => new RestClient
            {
                BaseUrl = new Uri(apiEndpoint),
                Proxy = new WebProxy(proxyAddress)
            };

        private string Encode(IDictionary<string, object> data)
        {
            var totalMilisecondNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            data.Add("iat", totalMilisecondNow);
            data.Add("exp", totalMilisecondNow + ExpiredTokenPeriodInMiliseconds);

            return jwtHelper.Encode(messengerSettings.SecureKey, data);
        }

        private static string EncodeBase64(string input)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }
}