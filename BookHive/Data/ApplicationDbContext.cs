using BookHive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Book> Books => Set<Book>();

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(user => user.FirstName).HasMaxLength(100);
            entity.Property(user => user.LastName).HasMaxLength(100);
            entity.Property(user => user.Address).HasMaxLength(500);
            entity.Property(user => user.ProfileImagePath).HasMaxLength(255);
        });

        builder.Entity<IdentityRole>(entity => entity.ToTable("Roles"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));

        builder.Entity<Book>(ConfigureBook);
        builder.Entity<Cart>(ConfigureCart);
        builder.Entity<CartItem>(ConfigureCartItem);
        builder.Entity<Order>(ConfigureOrder);
        builder.Entity<OrderItem>(ConfigureOrderItem);
    }

    private static void ConfigureBook(EntityTypeBuilder<Book> entity)
    {
        entity.ToTable("Books");
        entity.Property(book => book.Title).HasMaxLength(200).IsRequired();
        entity.Property(book => book.Author).HasMaxLength(150).IsRequired();
        entity.Property(book => book.Category).HasMaxLength(100).IsRequired();
        entity.Property(book => book.Description).HasMaxLength(4000).IsRequired();
        entity.Property(book => book.CoverImagePath).HasMaxLength(255);
        entity.Property(book => book.Price).HasPrecision(18, 2);
    }

    private static void ConfigureCart(EntityTypeBuilder<Cart> entity)
    {
        entity.ToTable("Carts");
        entity.Property(cart => cart.ApplicationUserId).HasMaxLength(450).IsRequired();

        entity.HasOne(cart => cart.User)
            .WithOne(user => user.Cart)
            .HasForeignKey<Cart>(cart => cart.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureCartItem(EntityTypeBuilder<CartItem> entity)
    {
        entity.ToTable("CartItems");
        entity.Property(item => item.UnitPrice).HasPrecision(18, 2);

        entity.HasOne(item => item.Cart)
            .WithMany(cart => cart.Items)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(item => item.Book)
            .WithMany(book => book.CartItems)
            .HasForeignKey(item => item.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureOrder(EntityTypeBuilder<Order> entity)
    {
        entity.ToTable("Orders");
        entity.Property(order => order.ApplicationUserId).HasMaxLength(450).IsRequired();
        entity.Property(order => order.ShippingAddress).HasMaxLength(500).IsRequired();
        entity.Property(order => order.TotalAmount).HasPrecision(18, 2);
        entity.Property(order => order.Status).HasConversion<string>().HasMaxLength(50);

        entity.HasOne(order => order.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(order => order.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureOrderItem(EntityTypeBuilder<OrderItem> entity)
    {
        entity.ToTable("OrderItems");
        entity.Property(item => item.BookTitle).HasMaxLength(200).IsRequired();
        entity.Property(item => item.UnitPrice).HasPrecision(18, 2);

        entity.HasOne(item => item.Order)
            .WithMany(order => order.Items)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(item => item.Book)
            .WithMany(book => book.OrderItems)
            .HasForeignKey(item => item.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
