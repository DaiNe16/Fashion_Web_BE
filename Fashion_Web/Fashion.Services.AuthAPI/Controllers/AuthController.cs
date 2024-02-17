using Fashion.Services.AuthAPI.Data;
using Fashion.Services.AuthAPI.Models;
using Fashion.Services.AuthAPI.Models.Dto;
using Fashion.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Fashion.Services.AuthAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private ResponseDto _response;
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private IWebHostEnvironment _environment;
		public AuthController(IAuthService authService, AppDbContext appDbContext, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, RoleManager<IdentityRole> roleManager)
		{
			_authService = authService;
			_response = new ResponseDto();
			_db = appDbContext;
			_userManager = userManager;
			_environment = environment;
			_roleManager = roleManager;
		}

		[HttpGet("GetAllUser")]
		public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
		{
			try
			{
				var users = await _userManager.Users.ToListAsync();
				List<UserDto> listUserDto = new List<UserDto>();
				foreach (var user in users)
				{
					var roles = await _userManager.GetRolesAsync(user);
					UserDto userDto = new UserDto();
					userDto.ID = user.Id;
					userDto.Name = user.Name;
					userDto.Email = user.Name;
					userDto.PhoneNumber = user.PhoneNumber;
					userDto.AvatarUrl = user.AvatarUrl;
					userDto.Roles = roles;
					listUserDto.Add(userDto);
				}
				_response.Result = listUserDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return Ok(_response);
		}

		//($"/api/auth/GetUserById/"+id);
		[HttpGet("GetUserById/{id}")]
		public async Task<IActionResult> Login(string id)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);	
			if (user == null)
			{
				_response.IsSuccess = false;
				_response.Message = "User id is not existed";
				return BadRequest(_response);
			}

			_response.Result = user;
			return Ok(_response);
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
		{
			var loginResponse = await _authService.Login(loginRequestDto);
			if (loginResponse.User == null)
			{
				_response.IsSuccess = false;
				_response.Message = "Username or password is incorrect";
				return BadRequest(_response);
			}

			_response.Result = loginResponse;
			return Ok(_response);
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
		{
			var registrationResponse = await _authService.Register(registrationRequestDto);
			if (!string.IsNullOrEmpty(registrationResponse))
			{
				_response.IsSuccess = false;
				_response.Message = registrationResponse;
				return BadRequest(_response);
			}

			_response.Result = "Account has been registered.";
			return Ok(_response);
		}

		[HttpPost("AssignRole")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> AssignRole(string email, string roleName)
		{
			var response = await _authService.AssignRole(email, roleName);
			if (!response)
			{
				_response.IsSuccess = false;
				_response.Message = "Failed to assign role.";
				return BadRequest(_response);
			}
			_response.Message = "Successfully to assign role.";
			return Ok(_response);
		}

		[HttpGet("GetMe")]
		[Authorize]
		public async Task<ActionResult<UserDto>> GetMe()
		{
			UserDto userDto = new UserDto();
			// Access claims from ClaimsPrincipal
			//var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
			//var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;

			var user_DB = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == username);
			if (user_DB == null)
			{
				return NotFound(userDto);
			}
			var roles = await _userManager.GetRolesAsync(user_DB);

			userDto.ID = user_DB.Id;
			userDto.Name = user_DB.Name;
			userDto.Email = user_DB.Name;
			userDto.PhoneNumber = user_DB.PhoneNumber;
			userDto.AvatarUrl = user_DB.AvatarUrl;
			userDto.Roles = roles;

			// Access roles from ClaimsPrincipal
			//var roles = User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList();

			return Ok(userDto);
		}

		[HttpPost("ChangePassword")]
		[Authorize]
		public async Task<ActionResult> ChangePassword(string OldPassword, string NewPassword, string ReTypeNewPassword)
		{
			var result = await _authService.ChangePassword(OldPassword, NewPassword, ReTypeNewPassword);
			if(result.Succeeded)
			{
				_response.Message = "Change password successfully.";
			}
			else
			{
				_response.IsSuccess = false;
				_response.Message = result.Errors.FirstOrDefault()?.Description;
			}
			return Ok(_response);
		}

		[HttpPost("UpdateAvatar")]
		[Authorize]
		public async Task<ActionResult> UpdateAvatar(IFormFile source)
		{
			try
			{
				var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;

				var user_DB = _db.ApplicationUsers.AsNoTracking().FirstOrDefault(u => u.UserName == username);
				if (user_DB == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Can not find user.";
					return NotFound(_response);
				}

				string fileName = source.FileName;
				string folderPath = GetFolderPath(user_DB.Id);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				//string imagePath = "";
				//if (fileName.EndsWith(".png")){
				//    imagePath = folderPath + "\\image.png";
				//}
				//else if (fileName.EndsWith(".jpg"))
				//{
				//    imagePath = folderPath + "\\image.jpg";
				//}
				//else
				//{
				//    _response.IsSuccess = false;
				//    _response.Message = "Type of image is invalid.";
				//    return _response;
				//}
				string imagePath = folderPath + "\\image.png";
				if (System.IO.File.Exists(imagePath))
				{
					System.IO.File.Delete(imagePath);
				}
				using (FileStream stream = System.IO.File.Create(imagePath))
				{
					await source.CopyToAsync(stream);
				}
				user_DB.AvatarUrl = GetImagePathByUserId(user_DB.Id);
				_db.ApplicationUsers.Update(user_DB);
				_db.SaveChanges();

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return Ok(_response);
		}

		[NonAction]
		private string GetFolderPath(string user_id)
		{
			return _environment.WebRootPath + "\\Uploads\\UserAvatar\\" + user_id;
		}

		[NonAction]
		private string GetImagePathByUserId(string user_id)
		{
			string hostPath = "https://localhost:7001";
			string imagePath = GetFolderPath(user_id) + "\\image.png";
			string imageUrl = "";
			if (System.IO.File.Exists(imagePath))
			{
				imageUrl = hostPath + "/Uploads/UserAvatar/" + user_id + "/image.png";
			}
			return imageUrl;
		}


	}
}
