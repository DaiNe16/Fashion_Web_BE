using Fashion.Services.AuthAPI.Data;
using Fashion.Services.AuthAPI.Models;
using Fashion.Services.AuthAPI.Models.Dto;
using Fashion.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Fashion.Services.AuthAPI.Service
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
			UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
		{
			_db = db;
			_jwtTokenGenerator = jwtTokenGenerator;
			_userManager = userManager;
			_roleManager = roleManager;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<bool> AssignRole(string email, string roleName)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == email);
			if (user != null)
			{
				if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
				{
					//Create role if it does not exist
					_roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
				}
				await _userManager.AddToRoleAsync(user, roleName);
				return true;
			}
			return false;
		}

		public async Task<IdentityResult> ChangePassword(string OldPassword, string NewPassword, string RetypeNewPassword)
		{
			var username = _httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
			var user_DB = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == username);

			if (user_DB == null)
			{
				// User not found
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var checkPass = await _userManager.CheckPasswordAsync(user_DB, OldPassword);
			if (!checkPass)
			{
				// Current password is incorrect
				return IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect." });
			}

			if(NewPassword != RetypeNewPassword)
			{
				// Newpass is not equal Retype new pass
				return IdentityResult.Failed(new IdentityError { Description = "NewPassword is not equal RetypeNewPassword." });
			}

			var changePasswordResult = _userManager.ChangePasswordAsync(user_DB, OldPassword, NewPassword).GetAwaiter().GetResult();
			if(changePasswordResult.Succeeded)
			{
				return IdentityResult.Success;
			}
			else
			{
				// Password change failed
				return changePasswordResult;
			}
		}

		public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == loginRequestDto.UserName.ToLower());
			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

			if (user == null || !isValid)
			{
				return new LoginResponseDto() { User = null, Token = "" };
			}

			//if user was found , Generate JWT Token
			var roles = await _userManager.GetRolesAsync(user);
			var token = _jwtTokenGenerator.GenerateToken(user, roles);

			UserDto userDTO = new()
			{
				Email = user.Email,
				ID = user.Id,
				Name = user.Name,
				PhoneNumber = user.PhoneNumber
			};

			LoginResponseDto loginResponseDto = new LoginResponseDto()
			{
				User = userDTO,
				Token = token
			};

			return loginResponseDto;
		}

		public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
		{
			ApplicationUser user = new ApplicationUser()
			{
				UserName = registrationRequestDto.Name,
				Email = registrationRequestDto.Email,
				NormalizedEmail = registrationRequestDto.Email,
				Name = registrationRequestDto.Name,
				PhoneNumber = registrationRequestDto.PhoneNumber,
				AvatarUrl = "",
			};
			try
			{
				var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

				if (result.Succeeded)
				{
					var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registrationRequestDto.Name);
					UserDto userDto = new()
					{
						Email = userToReturn.Email,
						ID = userToReturn.Id,
						Name = userToReturn.Name,
						PhoneNumber = userToReturn.PhoneNumber
					};


					return "";
				}
				else
				{
					return result.Errors.FirstOrDefault().Description;
				}
			}
			catch (Exception ex) { }



			return "Error encountered";
		}
	}
}
