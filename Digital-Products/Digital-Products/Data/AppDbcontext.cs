using Microsoft.EntityFrameworkCore;
using Digital_Products.Models;
namespace Digital_Products.Data
{
    public class AppDbcontext  : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext>options) : base (options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); 

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
        }


        DbSet<User> Users { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<ShippingInfo> ShippingInfos { get; set; }
        DbSet<CartItem> CartItems { get; set; } 


    }
}
