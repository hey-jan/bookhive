using BookHive.Models;
using BookHive.Services;
using BookHive.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookHive.Controllers;

[Authorize]
public class CartController(UserManager<ApplicationUser> userManager, CartService cartService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);

        return View(new CartViewModel
        {
            Items = cart.Items.Select(item => new CartItemViewModel
            {
                Id = item.Id,
                BookId = item.BookId,
                Title = item.Book?.Title ?? "Unknown book",
                Author = item.Book?.Author ?? "Unknown author",
                CoverImagePath = item.Book?.CoverImagePath,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int bookId, int quantity = 1, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();
        try
        {
            await cartService.AddItemAsync(user.Id, bookId, Math.Max(quantity, 1), cancellationToken);
            TempData["StatusMessage"] = "Book added to your cart.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["StatusMessage"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        try
        {
            await cartService.UpdateQuantityAsync(user.Id, cartItemId, quantity, cancellationToken);
            TempData["StatusMessage"] = "Cart updated.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["StatusMessage"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        try
        {
            await cartService.RemoveItemAsync(user.Id, cartItemId, cancellationToken);
            TempData["StatusMessage"] = "Item removed from cart.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["StatusMessage"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var user = await userManager.GetUserAsync(User);
        return user ?? throw new InvalidOperationException("The current user could not be loaded.");
    }
}
