using BookHive.Models;
using BookHive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookHive.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/cart")]
public class CartApiController(
    UserManager<ApplicationUser> userManager,
    CartService cartService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);

        return Ok(CreateSummary(cart, "Cart loaded."));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromForm] int bookId, [FromForm] int quantity = 1, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();

        try
        {
            await cartService.AddItemAsync(user.Id, bookId, Math.Max(quantity, 1), cancellationToken);
            var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);
            return Ok(CreateSummary(cart, "Book added to your cart."));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] CartItemQuantityRequest request, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();

        try
        {
            await cartService.UpdateQuantityAsync(user.Id, cartItemId, request.Quantity, cancellationToken);
            var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);
            return Ok(CreateSummary(cart, "Cart updated."));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> DeleteItem(int cartItemId, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();

        try
        {
            await cartService.RemoveItemAsync(user.Id, cartItemId, cancellationToken);
            var cart = await cartService.GetOrCreateCartAsync(user.Id, cancellationToken);
            return Ok(CreateSummary(cart, "Item removed from cart."));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private static object CreateSummary(Cart cart, string message)
    {
        return new
        {
            message,
            itemCount = cart.Items.Sum(item => item.Quantity),
            totalAmount = cart.Items.Sum(item => item.UnitPrice * item.Quantity)
        };
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var user = await userManager.GetUserAsync(User);
        return user ?? throw new InvalidOperationException("The current user could not be loaded.");
    }

    public class CartItemQuantityRequest
    {
        public int Quantity { get; set; }
    }
}
