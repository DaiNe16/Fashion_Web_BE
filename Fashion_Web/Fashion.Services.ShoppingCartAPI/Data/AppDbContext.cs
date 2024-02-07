using Fashion.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Services.ShoppingCartAPI.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<CartHeader> CartHeader { get; set; }
		public DbSet<CartDetails> CartDetails { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

	}
}
