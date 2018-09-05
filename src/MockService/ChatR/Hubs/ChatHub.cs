namespace ChatR.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using ChatR.Helpers;
    using ChatR.Models;
    using ChatR.Utils;
    using Microsoft.AspNet.SignalR;

    public class ChatHub : Hub
    {
        private InMemoryRepository _repository;
        private IRestClientHelper _restClientHelper;

        public ChatHub()
        {
            _repository = InMemoryRepository.GetInstance();
            _restClientHelper = new RestClientHelper();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userId = _repository.GetUserByConnectionId(Context.ConnectionId);
            if (userId != null)
            {
                ChatUser user = _repository.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    _repository.Remove(user);
                    return Clients.All.leaves(user.Id, user.Username, DateTime.Now);
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public void Send(ChatMessage message)
        {
            if (!string.IsNullOrEmpty(message.Content))
            {
                ShowMessage(message);

                if (message.Content.StartsWith("#"))
                {
                    var contents = message.Content.TrimStart('#').Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (contents[0].Equals("message"))
                    {
                        PushMessageEvent(message.Username, contents[1]);
                    }
                    else
                    {
                        PushWebHooksEvent(message.Username, contents[0]);
                    }
                }
                else
                {
                    PushMessageEvent(message.Username, message.Content);
                }
            }
        }

        public void Receive(ChatMessage message)
        {
            if (!string.IsNullOrEmpty(message.Content))
            {
                ShowMessage(message);
            }
        }

        public void Joined()
        {
            ChatUser user = new ChatUser()
            {
                Id = Guid.NewGuid().ToString(),
                Username = Clients.Caller.username
            };
            _repository.Add(user);
            _repository.AddMapping(Context.ConnectionId, user.Id);
            Clients.All.joins(user.Id, Clients.Caller.username, DateTime.Now);
        }

        public ICollection<ChatUser> GetConnectedUsers()
        {
            return _repository.Users.ToList<ChatUser>();
        }

        private void ShowMessage(ChatMessage message)
        {
            //message.Content = HttpUtility.HtmlEncode(message.Content);
            HashSet<string> extractedURLs;
            message.Content = TextParser.TransformAndExtractUrls(message.Content, out extractedURLs);
            message.Timestamp = DateTime.Now;

            Clients.All.onMessageReceived(message);
        }

        private Result PushMessageEvent(string userId, string content)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(content), content);

            return _restClientHelper.Execute<Result>(
                "/incoming/letstalk",
                RestSharp.Method.POST,
                new
                {
                    type = "message",
                    timestamp = 1527151205790,
                    source = new Source
                    {
                        Type = "user",
                        UserId = userId
                    },
                    payload = content
                });
        }

        private Result PushWebHooksEvent(string userId, string eventName)
        {
            return _restClientHelper.Execute<Result>(
                "/incoming/letstalk",
                RestSharp.Method.POST,
                new
                {
                    type = eventName,
                    timestamp = 1527151205790,
                    source = new Source
                    {
                        Type = "user",
                        UserId = userId
                    },
                });
        }

        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}