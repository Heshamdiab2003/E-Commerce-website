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


            modelBuilder.Entity<CartItem>()
    .HasOne(c => c.Order)
    .WithMany(o => o.CartItems)
    .HasForeignKey(c => c.OrderId)
    .IsRequired(false); // ده هو المفتاح لحل المشكلة


        }


        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShippingInfo> ShippingInfos { get; set; }
        public DbSet<CartItem> CartItems { get; set; } 


    }
}
