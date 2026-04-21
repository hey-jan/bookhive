using BookHive.Data;
using BookHive.Models;
using BookHive.Services;
using BookHive.ViewModels.Cart;
using BookHive.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Controllers;

[Authorize]
public class OrdersController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    CartService cartService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);
        var model = BuildCheckoutViewModel(cart, user.Address);

        if (!model.Items.Any())
        {
            TempData["StatusMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);

        if (!cart.Items.Any())
        {
            ModelState.AddModelError(string.Empty, "Your cart is empty.");
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = BuildCheckoutViewModel(cart, model.ShippingAddress);
            return View(invalidModel);
        }

        foreach (var item in cart.Items)
        {
            if (item.Book is null || item.Book.InventoryQuantity < item.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"Insufficient stock for {item.Book?.Title ?? "a selected book"}.");
            }
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = BuildCheckoutViewModel(cart, model.ShippingAddress);
            return View(invalidModel);
        }

        var order = new Order
        {
            ApplicationUserId = user.Id,
            ShippingAddress = model.ShippingAddress,
            TotalAmount = cart.Items.Sum(item => item.UnitPrice * item.Quantity),
            Status = OrderStatus.Processing,
            Items = cart.Items.Select(item =>
            {
                item.Book!.InventoryQuantity -= item.Quantity;

                return new OrderItem
                {
                    BookId = item.BookId,
                    BookTitle = item.Book.Title,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
            }).ToList()
        };

        user.Address = model.ShippingAddress;
        dbContext.Orders.Add(order);
        dbContext.CartItems.RemoveRange(cart.Items);
        cart.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Summary), new { id = order.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Summary(int id, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        var order = await dbContext.Orders
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.Id == id && item.ApplicationUserId == user.Id, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return View(new OrderSummaryViewModel
        {
            OrderId = order.Id,
            ShippingAddress = order.ShippingAddress,
            PlacedAtUtc = order.PlacedAtUtc,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(item => new OrderSummaryItemViewModel
            {
                BookTitle = item.BookTitle,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        });
    }

    private static CheckoutViewModel BuildCheckoutViewModel(Cart cart, string? shippingAddress)
    {
        var items = cart.Items.Select(item => new CartItemViewModel
        {
            Id = item.Id,
            BookId = item.BookId,
            Title = item.Book?.Title ?? "Unknown book",
            Author = item.Book?.Author ?? "Unknown author",
            CoverImagePath = item.Book?.CoverImagePath,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList();

        return new CheckoutViewModel
        {
            ShippingAddress = shippingAddress ?? string.Empty,
            Items = items,
            TotalAmount = items.Sum(item => item.LineTotal)
        };
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var user = await userManager.GetUserAsync(User);
        return user ?? throw new InvalidOperationException("The current user could not be loaded.");
    }
}
