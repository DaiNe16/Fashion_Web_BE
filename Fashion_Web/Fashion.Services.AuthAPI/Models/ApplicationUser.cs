using Microsoft.AspNetCore.Identity;

namespace Fashion.Services.AuthAPI.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }
        public string AvatarUrl { get; set; }
    }
}
