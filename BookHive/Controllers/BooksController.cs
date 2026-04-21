using BookHive.Data;
using BookHive.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Controllers;

public class BooksController(ApplicationDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? category, decimal? minPrice, decimal? maxPrice)
    {
        var booksQuery = dbContext.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            booksQuery = booksQuery.Where(book =>
                book.Title.Contains(searchTerm) ||
                book.Author.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            booksQuery = booksQuery.Where(book => book.Category == category);
        }

        if (minPrice.HasValue)
        {
            booksQuery = booksQuery.Where(book => book.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            booksQuery = booksQuery.Where(book => book.Price <= maxPrice.Value);
        }

        var books = await booksQuery
            .OrderBy(book => book.Title)
            .ToListAsync();

        var categories = await dbContext.Books
            .Select(book => book.Category)
            .Distinct()
            .OrderBy(item => item)
            .ToListAsync();

        return View(new BookCatalogViewModel
        {
            Books = books,
            Categories = categories,
            SearchTerm = searchTerm,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var book = await dbContext.Books
            .FirstOrDefaultAsync(item => item.Id == id);

        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }
}
