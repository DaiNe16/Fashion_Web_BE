namespace Fashion.Services.ChatAPI.Models
{
	public class Chat
	{
        public int ChatId { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        public string ChatName { get; set; }
        public virtual IEnumerable<Message> Messages { get; set; }
    }
}
