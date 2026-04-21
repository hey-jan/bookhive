namespace BookHive.ViewModels.Cart;

public class CartViewModel
{
    public IReadOnlyCollection<CartItemViewModel> Items { get; set; } = [];

    public decimal TotalAmount => Items.Sum(item => item.LineTotal);
}
