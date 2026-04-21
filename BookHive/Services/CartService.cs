using BookHive.Data;
using BookHive.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Services;

public class CartService(ApplicationDbContext dbContext)
{
    public async Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await dbContext.Carts
            .Include(item => item.Items)
            .ThenInclude(item => item.Book)
            .FirstOrDefaultAsync(item => item.ApplicationUserId == userId, cancellationToken);

        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart
        {
            ApplicationUserId = userId
        };

        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.Carts
            .Include(item => item.Items)
            .ThenInclude(item => item.Book)
            .FirstAsync(item => item.Id == cart.Id, cancellationToken);
    }

    public async Task AddItemAsync(string userId, int bookId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be at least 1.");
        }

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var book = await dbContext.Books.FirstOrDefaultAsync(item => item.Id == bookId, cancellationToken)
            ?? throw new InvalidOperationException("The selected book could not be found.");

        var existingItem = cart.Items.FirstOrDefault(item => item.BookId == bookId);
        var requestedQuantity = quantity + (existingItem?.Quantity ?? 0);
        if (requestedQuantity > book.InventoryQuantity)
        {
            throw new InvalidOperationException($"Only {book.InventoryQuantity} copies of \"{book.Title}\" are currently in stock.");
        }

        if (existingItem is null)
        {
            dbContext.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                BookId = bookId,
                Quantity = quantity,
                UnitPrice = book.Price
            });
        }
        else
        {
            existingItem.Quantity += quantity;
            existingItem.UnitPrice = book.Price;
        }

        cart.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateQuantityAsync(string userId, int cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var item = cart.Items.FirstOrDefault(entry => entry.Id == cartItemId)
            ?? throw new InvalidOperationException("The selected cart item could not be found.");

        if (quantity <= 0)
        {
            dbContext.CartItems.Remove(item);
        }
        else
        {
            var book = item.Book ?? await dbContext.Books.FirstOrDefaultAsync(book => book.Id == item.BookId, cancellationToken)
                ?? throw new InvalidOperationException("The selected book could not be found.");

            if (quantity > book.InventoryQuantity)
            {
                throw new InvalidOperationException($"Only {book.InventoryQuantity} copies of \"{book.Title}\" are currently in stock.");
            }

            item.Quantity = quantity;
        }

        cart.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveItemAsync(string userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var item = cart.Items.FirstOrDefault(entry => entry.Id == cartItemId)
            ?? throw new InvalidOperationException("The selected cart item could not be found.");

        dbContext.CartItems.Remove(item);
        cart.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
