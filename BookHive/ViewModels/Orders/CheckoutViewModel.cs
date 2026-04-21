using BookHive.ViewModels.Cart;
using System.ComponentModel.DataAnnotations;

namespace BookHive.ViewModels.Orders;

public class CheckoutViewModel
{
    [Required]
    [StringLength(500)]
    [Display(Name = "Shipping address")]
    public string ShippingAddress { get; set; } = string.Empty;

    public IReadOnlyCollection<CartItemViewModel> Items { get; set; } = [];

    public decimal TotalAmount { get; set; }
}
