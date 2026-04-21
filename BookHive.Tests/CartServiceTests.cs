using BookHive.Data;
using BookHive.Models;
using BookHive.Services;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Tests;

public class CartServiceTests
{
    [Fact]
    public async Task GetOrCreateCartAsync_CreatesCartWhenMissing()
    {
        await using var dbContext = CreateDbContext();
        var service = new CartService(dbContext);

        var cart = await service.GetOrCreateCartAsync("user-1");

        Assert.NotEqual(0, cart.Id);
        Assert.Equal("user-1", cart.ApplicationUserId);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task AddItemAsync_ThrowsWhenRequestedQuantityExceedsInventory()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Books.Add(new Book
        {
            Id = 1,
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Category = "Architecture",
            Price = 59.99m,
            Description = "A software design reference.",
            InventoryQuantity = 2
        });
        await dbContext.SaveChangesAsync();

        var service = new CartService(dbContext);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddItemAsync("user-1", 1, 3));

        Assert.Contains("currently in stock", exception.Message);
    }

    [Fact]
    public async Task UpdateQuantityAsync_RemovesItemWhenQuantityIsZero()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Books.Add(new Book
        {
            Id = 1,
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Category = "Engineering",
            Price = 45.50m,
            Description = "Code craftsmanship practices.",
            InventoryQuantity = 5
        });
        await dbContext.SaveChangesAsync();

        var service = new CartService(dbContext);
        await service.AddItemAsync("user-1", 1, 1);

        var cart = await service.GetOrCreateCartAsync("user-1");
        var itemId = Assert.Single(cart.Items).Id;

        await service.UpdateQuantityAsync("user-1", itemId, 0);

        var updatedCart = await service.GetOrCreateCartAsync("user-1");
        Assert.Empty(updatedCart.Items);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new ApplicationDbContext(options);
    }
}
