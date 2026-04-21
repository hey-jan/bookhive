namespace BookHive.ViewModels.Admin;

public class AdminInventoryViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public int InventoryQuantity { get; set; }
}
