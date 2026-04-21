using BookHive.Models;

namespace BookHive.ViewModels.Books;

public class BookCatalogViewModel
{
    public IReadOnlyCollection<Book> Books { get; set; } = [];

    public IReadOnlyCollection<string> Categories { get; set; } = [];

    public string? SearchTerm { get; set; }

    public string? Category { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
}
