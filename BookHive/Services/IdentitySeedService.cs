using BookHive.Models;
using Microsoft.AspNetCore.Identity;

namespace BookHive.Services;

public static class IdentitySeedService
{
    private static readonly string[] Roles = ["Admin", "Customer"];

    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = configuration["Admin:Email"];
        var adminPassword = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "BookHive",
                LastName = "Admin"
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createAdminResult.Succeeded)
            {
                var errors = string.Join(", ", createAdminResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Unable to seed admin account: {errors}");
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
        {
            await userManager.AddToRoleAsync(existingAdmin, "Admin");
        }
    }
}
