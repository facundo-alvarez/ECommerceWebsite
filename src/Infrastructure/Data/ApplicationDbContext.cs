using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User_Product>()
                .HasOne(p => p.Product)
                .WithMany(up => up.User_Product)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<User_Product>()
                .HasOne(p => p.User)
                .WithMany(up => up.User_Product)
                .HasForeignKey(p => p.UserId);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<User_Product> User_Products { get; set; }
    }
}
