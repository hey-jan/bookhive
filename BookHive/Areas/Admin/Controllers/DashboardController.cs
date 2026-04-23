using BookHive.Data;
using BookHive.Models;
using BookHive.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController(ApplicationDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new AdminDashboardViewModel
        {
            UserCount = await dbContext.Users.CountAsync(cancellationToken),
            OrderCount = await dbContext.Orders.CountAsync(cancellationToken),
            LowStockCount = await dbContext.Books.CountAsync(book => book.InventoryQuantity <= 5, cancellationToken),
            RevenueTotal = await dbContext.Orders.SumAsync(order => (decimal?)order.TotalAmount, cancellationToken) ?? 0m,
            Users = await (
                from user in dbContext.Users
                join userRole in dbContext.Set<IdentityUserRole<string>>() on user.Id equals userRole.UserId
                join role in dbContext.Roles on userRole.RoleId equals role.Id
                where role.Name == "Customer"
                orderby user.CreatedAtUtc descending
                select new AdminUserViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? string.Empty,
                    CreatedAtUtc = user.CreatedAtUtc
                })
                .Take(8)
                .ToListAsync(cancellationToken),
            Orders = await dbContext.Orders
                .Include(order => order.User)
                .OrderByDescending(order => order.PlacedAtUtc)
                .Take(10)
                .Select(order => new AdminOrderViewModel
                {
                    Id = order.Id,
                    CustomerEmail = order.User!.Email ?? string.Empty,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    PlacedAtUtc = order.PlacedAtUtc
                })
                .ToListAsync(cancellationToken),
            LowStockBooks = await dbContext.Books
                .Where(book => book.InventoryQuantity <= 5)
                .OrderBy(book => book.InventoryQuantity)
                .ThenBy(book => book.Title)
                .Select(book => new AdminInventoryViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    InventoryQuantity = book.InventoryQuantity
                })
                .ToListAsync(cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(item => item.Id == orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        order.Status = status;
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["StatusMessage"] = $"Updated order #{order.Id} to {status}.";
        return RedirectToAction(nameof(Index));
    }
}
