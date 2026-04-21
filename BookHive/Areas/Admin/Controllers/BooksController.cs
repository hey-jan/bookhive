using BookHive.Data;
using BookHive.Models;
using BookHive.Services;
using BookHive.ViewModels.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BooksController(ApplicationDbContext dbContext, IFileStorageService fileStorageService) : Controller
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
    public IActionResult Create()
    {
        return View(new BookFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var coverImagePath = await fileStorageService.SaveFileAsync(model.CoverImage, "books", cancellationToken);

        var book = new Book
        {
            Title = model.Title,
            Author = model.Author,
            Category = model.Category,
            Price = model.Price,
            Description = model.Description,
            InventoryQuantity = model.InventoryQuantity,
            CoverImagePath = coverImagePath
        };

        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["StatusMessage"] = $"Added \"{book.Title}\" to the catalog.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await dbContext.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return View(new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Price = book.Price,
            Description = book.Description,
            InventoryQuantity = book.InventoryQuantity,
            ExistingCoverImagePath = book.CoverImagePath
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var book = await dbContext.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        if (model.CoverImage is not null)
        {
            fileStorageService.DeleteFile(book.CoverImagePath);
            book.CoverImagePath = await fileStorageService.SaveFileAsync(model.CoverImage, "books", cancellationToken);
        }

        book.Title = model.Title;
        book.Author = model.Author;
        book.Category = model.Category;
        book.Price = model.Price;
        book.Description = model.Description;
        book.InventoryQuantity = model.InventoryQuantity;
        book.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["StatusMessage"] = $"Updated \"{book.Title}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        fileStorageService.DeleteFile(book.CoverImagePath);
        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["StatusMessage"] = $"Deleted \"{book.Title}\" from the catalog.";
        return RedirectToAction(nameof(Index));
    }
}
