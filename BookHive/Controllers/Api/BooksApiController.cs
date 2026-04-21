using BookHive.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Controllers.Api;

[ApiController]
[Route("api/books")]
public class BooksApiController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBooks(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var booksQuery = dbContext.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            booksQuery = booksQuery.Where(book => book.Title.Contains(searchTerm) || book.Author.Contains(searchTerm));
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
            .Select(book => new
            {
                book.Id,
                book.Title,
                book.Author,
                book.Category,
                book.Price,
                book.Description,
                book.CoverImagePath,
                book.InventoryQuantity
            })
            .ToListAsync(cancellationToken);

        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBook(int id, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books
            .Where(item => item.Id == id)
            .Select(book => new
            {
                book.Id,
                book.Title,
                book.Author,
                book.Category,
                book.Price,
                book.Description,
                book.CoverImagePath,
                book.InventoryQuantity
            })
            .FirstOrDefaultAsync(cancellationToken);

        return book is null ? NotFound() : Ok(book);
    }
}
