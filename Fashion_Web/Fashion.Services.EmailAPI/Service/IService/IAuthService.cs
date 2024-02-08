using Fashion.Services.EmailAPI.Models.Dto;

namespace Fashion.Services.EmailAPI.Service.IService
{
	public interface IAuthService
	{
		Task<ApplicationUserDto> GetUserById(string id);
	}
}
