using Microsoft.EntityFrameworkCore;

namespace linkedin_Learning_Dot_Net_8.Models
{
	public class ShopContext : DbContext
	{
        public ShopContext(DbContextOptions<ShopContext> options) :base(options) { }
        
        public DbSet<Product> Products { get; set; } 
        public DbSet<Category> Categories { get; set; }
    }
}
