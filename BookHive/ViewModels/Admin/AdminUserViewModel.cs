namespace BookHive.ViewModels.Admin;

public class AdminUserViewModel
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
