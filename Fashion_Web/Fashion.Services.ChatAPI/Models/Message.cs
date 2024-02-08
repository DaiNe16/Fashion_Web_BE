using System.ComponentModel.DataAnnotations.Schema;

namespace Fashion.Services.ChatAPI.Models
{
	public class Message
	{
        public int MessageId { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
