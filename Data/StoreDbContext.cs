using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Data
{
    public class StoreDbContext : IdentityDbContext<User>
    {
        public StoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductVariantImage> ProductVariantImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderInfo> OrderInfos { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<ImageEvaluation> ImageEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Order>()
                .HasOne(o => o.OrderInfo)
                .WithMany(oi => oi.Orders)
                .HasForeignKey(o => o.OrderInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.CategoryChildren);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            SeedingData(builder);
        }

        public void SeedingData(ModelBuilder builder)
        {

            var roles = new List<IdentityRole>()
            {
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "Customer", ConcurrencyStamp = "1", NormalizedName = "Customer" }
            };


            // ROLE
            builder.Entity<IdentityRole>().HasData(roles);

            // USER
            var appUser = new User
            {
                FullName = "Đạo Thanh Hưng",
                Email = "cskhshop5@gmail.com",
                NormalizedEmail = "CSKHSHOP5@GMAIL.COM",
                EmailConfirmed = true,
                UserName = "hungdtadmin",
                NormalizedUserName = "HUNGDTADMIN",
                PhoneNumber = "0000000000",
                IsActive = true,
            };

            PasswordHasher<User> hashedPassword = new PasswordHasher<User>();
            appUser.PasswordHash = hashedPassword.HashPassword(appUser, "12345678");

            builder.Entity<User>().HasData(appUser);

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { UserId = appUser.Id, RoleId = roles[0].Id }
            );

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Men's Clothing", Description = "Clothing for men", IsActive = true },
                new Category { Id = 2, Name = "Women's Clothing", Description = "Clothing for women", IsActive = true },
                new Category { Id = 3, Name = "Kid's Clothing", Description = "Clothing for kids", IsActive = true },
                new Category { Id = 4, Name = "Footwear", Description = "Shoes and footwear", IsActive = true },
                new Category { Id = 5, Name = "Accessories", Description = "Fashion accessories", IsActive = true }
            };

            builder.Entity<Category>().HasData(categories);

            var brands = new List<Brand>
            {
                new Brand { Id = 1, Name = "Nike", Description = "Nike is a global brand known for its sports apparel and accessories.", IsActive = true },
                new Brand { Id = 2, Name = "Adidas", Description = "Adidas offers a range of sportswear, footwear, and accessories.", IsActive = true },
                new Brand { Id = 3, Name = "Puma", Description = "Puma is known for its athletic footwear, apparel, and accessories.", IsActive = true },
                new Brand { Id = 4, Name = "Levi's", Description = "Levi's is a popular brand for jeans and casual wear.", IsActive = true },
                new Brand { Id = 5, Name = "H&M", Description = "H&M provides a variety of fashion-forward clothing and accessories.", IsActive = true }
            };

            builder.Entity<Brand>().HasData(brands);

            var colors = new List<Color>
            {
                new Color { Id = 1, Name = "Red", HexCode = "#FF0000", IsActive = true },
                new Color { Id = 2, Name = "Blue", HexCode = "#0000FF", IsActive = true },
                new Color { Id = 3, Name = "Green", HexCode = "#00FF00", IsActive = true },
                new Color { Id = 4, Name = "Yellow", HexCode = "#FFFF00", IsActive = true },
                new Color { Id = 5, Name = "Black", HexCode = "#000000", IsActive = true }
            };

            builder.Entity<Color>().HasData(colors);

            var sizes = new List<Size>
            {
                new Size { Id = 1, ESize = "S", MinHeight = 150, MaxHeight = 165, MinWeight = 50, MaxWeight = 60, IsActive = true },
                new Size { Id = 2, ESize = "M", MinHeight = 165, MaxHeight = 175, MinWeight = 60, MaxWeight = 70, IsActive = true },
                new Size { Id = 3, ESize = "L", MinHeight = 175, MaxHeight = 185, MinWeight = 70, MaxWeight = 80, IsActive = true },
                new Size { Id = 4, ESize = "XL", MinHeight = 185, MaxHeight = 195, MinWeight = 80, MaxWeight = 90, IsActive = true },
                new Size { Id = 5, ESize = "XXL", MinHeight = 195, MaxHeight = 205, MinWeight = 90, MaxWeight = 100, IsActive = true }
            };

            builder.Entity<Size>().HasData(sizes);
        }


    }
}
