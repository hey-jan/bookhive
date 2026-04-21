using System.ComponentModel.DataAnnotations;

namespace BookHive.ViewModels.Books;

public class BookFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Display(Name = "Inventory quantity")]
    public int InventoryQuantity { get; set; }

    [Display(Name = "Cover image")]
    public IFormFile? CoverImage { get; set; }

    public string? ExistingCoverImagePath { get; set; }
}
