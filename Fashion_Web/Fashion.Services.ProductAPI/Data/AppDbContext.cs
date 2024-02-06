using Fashion.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Services.ProductAPI.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Category> Categories { get; set; }
		public DbSet<SubCategory> SubCategories { get; set; }
		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

	}
}
