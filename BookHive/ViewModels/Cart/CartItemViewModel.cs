namespace BookHive.ViewModels.Cart;

public class CartItemViewModel
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string? CoverImagePath { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
