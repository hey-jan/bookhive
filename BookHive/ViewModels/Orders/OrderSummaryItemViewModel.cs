namespace BookHive.ViewModels.Orders;

public class OrderSummaryItemViewModel
{
    public string BookTitle { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;
}
