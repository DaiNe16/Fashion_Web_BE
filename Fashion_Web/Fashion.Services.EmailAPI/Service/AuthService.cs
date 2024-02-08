using Fashion.Services.EmailAPI.Models.Dto;
using Fashion.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;

namespace Fashion.Services.EmailAPI.Service
{
	public class AuthService : IAuthService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public AuthService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		public async Task<ApplicationUserDto> GetUserById(string id)
		{
			if (id.Contains('"'))
			{
				id = id.Substring(1);
			}
			string url = "/api/auth/GetUserById/" + id;
			var client = _httpClientFactory.CreateClient("Auth");
			var response = await client.GetAsync(url);
			var apiContent = await response.Content.ReadAsStringAsync();
			var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
			if (resp != null && resp.IsSuccess)
			{
				return JsonConvert.DeserializeObject<ApplicationUserDto>(Convert.ToString(resp.Result));
			}
			return new ApplicationUserDto();
		}
	}
}
