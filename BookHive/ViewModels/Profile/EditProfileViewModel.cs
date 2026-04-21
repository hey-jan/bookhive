using System.ComponentModel.DataAnnotations;

namespace BookHive.ViewModels.Profile;

public class EditProfileViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Display(Name = "Profile image")]
    public IFormFile? ProfileImage { get; set; }

    public string? ExistingProfileImagePath { get; set; }
}
