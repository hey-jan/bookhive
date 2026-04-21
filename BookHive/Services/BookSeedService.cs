using System.Text.Json;
using BookHive.Data;
using BookHive.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Services;

public static class BookSeedService
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if (await dbContext.Books.AnyAsync())
        {
            return;
        }

        var seedFilePath = Path.Combine(environment.ContentRootPath, "Data", "Seed", "books.json");
        if (!File.Exists(seedFilePath))
        {
            throw new FileNotFoundException("The book seed data file could not be found.", seedFilePath);
        }

        await using var stream = File.OpenRead(seedFilePath);
        var seedBooks = await JsonSerializer.DeserializeAsync<List<SeedBook>>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (seedBooks is null || seedBooks.Count == 0)
        {
            return;
        }

        dbContext.Books.AddRange(seedBooks.Select(seedBook => new Book
        {
            Title = seedBook.Title,
            Author = seedBook.Author,
            Category = seedBook.Category,
            Price = seedBook.Price,
            Description = seedBook.Description,
            InventoryQuantity = seedBook.InventoryQuantity
        }));

        await dbContext.SaveChangesAsync();
    }
}
