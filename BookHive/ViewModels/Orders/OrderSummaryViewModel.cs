namespace BookHive.ViewModels.Orders;

public class OrderSummaryViewModel
{
    public int OrderId { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public DateTime PlacedAtUtc { get; set; }

    public decimal TotalAmount { get; set; }

    public IReadOnlyCollection<OrderSummaryItemViewModel> Items { get; set; } = [];
}
