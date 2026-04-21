using BookHive.Models;

namespace BookHive.ViewModels.Admin;

public class AdminOrderViewModel
{
    public int Id { get; set; }

    public string CustomerEmail { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime PlacedAtUtc { get; set; }
}
