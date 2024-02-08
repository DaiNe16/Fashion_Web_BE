using Fashion.Services.ChatAPI.Data;
using Fashion.Services.ChatAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace Fashion.Services.ChatAPI.Hubs
{
	public class ChatHub : Hub
	{
		//clientSide
		//const connection = new signalR.HubConnectionBuilder()
		//.withUrl("/chatHub?userId=YOUR_USER_ID_HERE")
		//.build();
		private readonly AppDbContext _db;
		private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

        public ChatHub(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        public async Task SendMessage(string user, string message)
		{
			string UserIdFrom = Context.GetHttpContext().Request.Query["userId"];
			if (userConnections.ContainsKey(user))
			{
				string connectionId = userConnections[user];
				await Clients.Client(connectionId).SendAsync("ReceiveMessage", UserIdFrom, message);
			}
			//Log to DB
			var chat = _db.Chats.Where(u => ((u.UserId1 == user && u.UserId2 == UserIdFrom) || (u.UserId1 == UserIdFrom && u.UserId2 == user))).FirstOrDefault();
			if(chat == null)
			{
				//There is no chat. Create a new chat
				Chat newChat = new Chat
				{
					UserId1 = UserIdFrom,
					UserId2 = user,
					ChatName = $"Chat between user {UserIdFrom} and {user}"
				};
				_db.Chats.Add(newChat);
				_db.SaveChanges();

				//Log to message table
				Message newMessage = new Message
				{
					ChatId = newChat.ChatId,
					FromUserName = UserIdFrom,
					ToUserName = user,
					MessageContent = message,
					CreatedAt = DateTime.Now,
				};
				_db.Messages.Add(newMessage);
				_db.SaveChanges();
			}
			else
			{
				//There is existed chat
				//Log to message table
				Message newMessage = new Message
				{
					ChatId = chat.ChatId,
					FromUserName = UserIdFrom,
					ToUserName = user,
					MessageContent = message,
					CreatedAt = DateTime.Now,
				};
				_db.Messages.Add(newMessage);
				_db.SaveChanges();
			}
		}

		public override async Task OnConnectedAsync()
		{
			string userId = Context.GetHttpContext().Request.Query["userId"];
			string connectionId = Context.ConnectionId;

			// Add user to dictionary
			if (!userConnections.ContainsKey(userId))
			{
				userConnections.Add(userId, connectionId);
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			string userId = Context.GetHttpContext().Request.Query["userId"];

			// Remove user from dictionary
			if (userConnections.ContainsKey(userId))
			{
				userConnections.Remove(userId);
			}

			await base.OnDisconnectedAsync(exception);
		}
	}
}
