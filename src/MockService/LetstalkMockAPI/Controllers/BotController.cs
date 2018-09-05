namespace LetstalkMockAPI.Controllers
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using LetstalkMockAPI.Configurations;
    using LetstalkMockAPI.Models;
    using LetstalkMockAPI.ViewModels;
    using LetstalkMockAPI.Web;
    using Microsoft.AspNet.SignalR.Client;
    using Newtonsoft.Json;

    public class BotController : ApiController
    {
        private IHubProxy chatRHubProxy;

        public BotController()
        {
            var hubConnection = new HubConnection(AppSettings.MessengerAppUrl);
            chatRHubProxy = hubConnection.CreateHubProxy("ChatHub");
            hubConnection.Start().Wait();
        }

        [HttpPut]
        [Route("api/bot/change_display_name/{account_id}")]
        public IHttpActionResult ChangeDisplayName(int account_id, [FromBody]ChangeDisplayNameViewModel body)
            => SuccessJsonResult();

        [HttpPost]
        [Route("api/bot/upload_avatar_image/{account_id}")]
        public IHttpActionResult UploadAvatarImage(int account_id, [FromBody]UploadAvatarImageViewModel body)
            => SuccessJsonResult();

        [HttpPost]
        [Route("api/bot/set_welcome_msg/{account_id}")]
        public IHttpActionResult SetWelcomeMsg(int account_id, [FromBody]SetWelcomeMsgViewModel body)
            => SuccessJsonResult();

        [HttpPost]
        [Route("api/bot/set_auto_reply/{account_id}")]
        public IHttpActionResult SetAutoReply(int account_id, [FromBody]SetAutoReplyViewModel body)
            => SuccessJsonResult();

        [HttpPost]
        public IHttpActionResult Push(int account_id, [FromBody]PushMessageViewModel body)
        {
            var request = HttpContext.Current.Request;

            chatRHubProxy.Invoke<ChatMessage>(
                "Receive",
                new ChatMessage
                {
                    AccountId = account_id,
                    Username = "Letstalk Bot",
                    Content = BuildContent(body)
                });

            return SuccessJsonResult();
        }

        private string BuildContent(PushMessageViewModel body)
        {
            var content = new StringBuilder();
            content.AppendLine($"<p>{body.Payload.HashContent}</p>");
            content.AppendLine($"<p><b>Alpha Bot API --> Messenger Backend Mock Service:</b></p>");

            content.AppendLine("<p><i>HEADERS</i></p>");
            var headers = HttpContext.Current.Request.Headers;
            content.AppendLine($"<p>&emsp;Content-Type: {headers["Content-Type"]}<br/>&emsp;Authorization: {headers["Authorization"]}</p>");

            content.AppendLine("<p><i>BODY</i></p>");
            var requestBody = JsonConvert.SerializeObject(body, Formatting.Indented)
                .Replace("\r\n", "<br/>")
                .Replace("    ", "&emsp;")
                .Replace("  ", "&ensp;");
            content.Append($"<p>{requestBody}</p>");

            return content.ToString();
        }

        /// <summary>
        /// To send message to tagged contacts.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "tag": "tag",
        ///         "payload": {
        ///             "id": 350,
        ///             "hash_content_key": "f922b4b70151303c1f85bd4e7712c0a033fa8ba7",
        ///             "time": 1527214433078,
        ///             "type": 0,
        ///             "hash_type": 2,
        ///             "user_id": "jimmy100.chen",
        ///             "hash_content": "x9CK80e2D2ulQX6+48AmzA==",
        ///             "from": 0,
        ///             "send_time": 1527214433177,
        ///             "due_date": 2788654433177,
        ///             "receiver": "jimmy.chen",
        ///             "hash_id": 1526294541848
        ///         }
        ///     }
        /// </remarks>
        /// <param name="account_id"></param>
        /// <param name="tag"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bot/push_to_tag/{account_id}/tag/{tag}")]
        public IHttpActionResult PushToTag(int account_id, string tag, [FromBody]PushToTagViewModel body)
            => SuccessJsonResult();

        [HttpPost, HttpDelete]
        [Route("api/bot/modify_tag/{account_id}")]
        public IHttpActionResult ModifyTag(int account_id, [FromBody]ModifyTagViewModel body)
            => SuccessJsonResult();

        [HttpGet]
        [Route("api/bot/list_tag/{account_id}")]
        public IHttpActionResult ListTag(int account_id)
            => new JsonResult(
                new ListTagResult
                {
                    Status = 1,
                    Message = "success",
                    Tags = new string[] { "tag1", "tag2" }
                },
                Request);

        [HttpGet]
        [Route("api/bot/get_contacts/{account_id}/tag/{tag}")]
        public IHttpActionResult GetContacts(int account_id, string tag)
            => new JsonResult(
                new GetContactsResult
                {
                    Status = 1,
                    Message = "success",
                    Tag = tag,
                    Contacts = new List<Contact>
                    {
                        new Contact {UserId="1",AvatarUrl="avatar_url", DisplayName="display_name" },
                        new Contact {UserId="2",AvatarUrl="avatar_url", DisplayName="display_name" }
                    }
                },
                Request);

        [HttpPost, HttpDelete]
        [Route("api/bot/modify_contact_tag/{account_id}")]
        public IHttpActionResult ModifyContactTag(int account_id, [FromBody]ModifyContactTagViewModel body)
            => SuccessJsonResult();

        [HttpPost]
        [Route("api/bot/set_hash_key/{account_id}")]
        public IHttpActionResult SetHashKey(int account_id, [FromBody]SetHashKeyViewModel body)
            => SuccessJsonResult();

        [HttpGet]
        [Route("api/bot/get_user_public_key/{account_id}/user/{user_id}")]
        public IHttpActionResult GetUserPublicKey(int account_id, string user_id)
            => new JsonResult(new PublicKeyResult { Status = 1, Message = "success", PublicKey = "public_key" }, Request);

        [HttpPost]
        [Route("api/bot/set_key_to_user/{account_id}/user/{user_id}")]
        public IHttpActionResult SetKeyToUser(int account_id, string user_id, [FromBody]SetKeyToContactViewModel body)
           => SuccessJsonResult();

        [HttpPost]
        [Route("api/bot/set_webhook/{account_id}")]
        public IHttpActionResult SetWebhook(int account_id, [FromBody]SetWebhookViewModel body)
           => SuccessJsonResult();

        [HttpPut]
        [Route("api/bot/set_webhook/{account_id}")]
        public IHttpActionResult ModifyWebhook(int account_id, [FromBody]SetWebhookViewModel body)
           => SuccessJsonResult();

        [HttpGet]
        [Route("api/bot/get_profile/{account_id}/user/{user_id}")]
        public IHttpActionResult GetProfile(int account_id, [FromBody]GetProfileViewModel body)
           => new JsonResult(
               new GetProfileResult
               {
                   Status = 1,
                   Message = "success",
                   AvartaUrl = "avatar_url",
                   DisplayName = "display_name"
               },
               Request);

        private JsonResult SuccessJsonResult()
            => new JsonResult(new SuccessResult(), Request);

        private JsonResult FailureJsonResult()
            => new JsonResult(new FailureResult(), Request);
    }
}