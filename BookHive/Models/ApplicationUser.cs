using Microsoft.AspNetCore.Identity;

namespace BookHive.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? ProfileImagePath { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Cart? Cart { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
