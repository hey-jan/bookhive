using BookHive.Models;
using BookHive.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookHive.Controllers;

[Authorize]
public class ProfileController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await GetCurrentUserAsync();
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await GetCurrentUserAsync();
        var model = new EditProfileViewModel
        {
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetCurrentUserAsync();
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = model.Email;
            user.UserName = model.Email;
            user.NormalizedEmail = null;
            user.NormalizedUserName = null;
        }

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = "Profile updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed()
    {
        var user = await GetCurrentUserAsync();
        var deleteResult = await userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            TempData["StatusMessage"] = "Unable to delete the account right now.";
            return RedirectToAction(nameof(Index));
        }

        await signInManager.SignOutAsync();
        TempData["StatusMessage"] = "Your account has been deleted.";

        return RedirectToAction("Index", "Home");
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var user = await userManager.GetUserAsync(User);
        return user ?? throw new InvalidOperationException("The current user could not be loaded.");
    }
}
