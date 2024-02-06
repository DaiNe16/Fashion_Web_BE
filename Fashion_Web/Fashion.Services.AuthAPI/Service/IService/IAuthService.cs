using Fashion.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace Fashion.Services.AuthAPI.Service.IService
{
	public interface IAuthService
	{
		Task<string> Register(RegistrationRequestDto registrationRequestDto);
		Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
		Task<bool> AssignRole(string email, string roleName);
		Task<IdentityResult> ChangePassword(string OldPassword, string NewPassword, string RetypeNewPassword);
	}
}
