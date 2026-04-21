using BookHive.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Controllers;

public class BooksController(ApplicationDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var books = await dbContext.Books
            .OrderBy(book => book.Title)
            .ToListAsync();

        return View(books);
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
