using System.ComponentModel.DataAnnotations.Schema;

namespace Fashion.Services.ChatAPI.Models.Dto
{
	public class MessageDto
	{
		public int MessageId { get; set; }
		public int ChatId { get; set; }
		public string FromUserName { get; set; }
		public string ToUserName { get; set; }
		public string MessageContent { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
