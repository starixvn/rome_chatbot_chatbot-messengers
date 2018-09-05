namespace LetstalkMockAPI.Web
{
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using LetstalkMockAPI.Models;

    public class JsonResult : IHttpActionResult
    {
        private readonly Result result;
        private readonly HttpRequestMessage request;

        public JsonResult(Result result, HttpRequestMessage request)
        {
            this.result = result;
            this.request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent<Result>(result, new JsonMediaTypeFormatter()),
                RequestMessage = request
            };

            return Task.FromResult(response);
        }
    }
}