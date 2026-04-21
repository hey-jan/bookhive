using System.ComponentModel.DataAnnotations;

namespace BookHive.ViewModels.Account;

public class RegisterViewModel
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

    [StringLength(20)]
    [Phone]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
