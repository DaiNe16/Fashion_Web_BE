using Fashion.Services.AuthAPI.Models;

namespace Fashion.Services.AuthAPI.Service.IService
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
	}
}
