namespace BookHive.Models;

public class Order
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
