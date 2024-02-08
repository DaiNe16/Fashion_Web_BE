using Fashion.Services.ChatAPI.Data;
using Fashion.Services.ChatAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fashion.Services.ChatAPI.Controllers
{
	[Route("api/chat")]
	[ApiController]
	public class ChatAPIController : ControllerBase
	{
		private ResponseDto _response;
		private readonly AppDbContext _db;

        public ChatAPIController(AppDbContext appDbContext)
        {
			_response = new ResponseDto();
			_db = appDbContext;
		}

		[HttpGet("GetChatByBothUserId")]
		public async Task<IActionResult> GetChatByBothUserId(string userId1, string userId2)
		{
			try
			{
				var chat = _db.Chats.Where(u => ((u.UserId1 == userId1 && u.UserId2 == userId2) || (u.UserId1 == userId2 && u.UserId2 == userId1))).FirstOrDefault();
				if (chat == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Chat is not existed";
					return BadRequest(_response);
				}

				_response.Result = chat;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return Ok(_response);
		}

		[HttpGet("GetAllMessageByChatId/{id}")]
		public async Task<IActionResult> GetAllMessageByChatId(int id)
		{
			try
			{
				var Messages = _db.Messages.Where(u => u.ChatId == id).ToList();
				_response.Result = Messages;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return Ok(_response);
		}
	}
}
