using Fashion.Services.ChatAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Services.ChatAPI.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Chat> Chats { get; set; }
		public DbSet<Message> Messages { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
		}

	}
}
