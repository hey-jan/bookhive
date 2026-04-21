namespace BookHive.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int UserCount { get; set; }

    public int OrderCount { get; set; }

    public int LowStockCount { get; set; }

    public decimal RevenueTotal { get; set; }

    public IReadOnlyCollection<AdminUserViewModel> Users { get; set; } = [];

    public IReadOnlyCollection<AdminOrderViewModel> Orders { get; set; } = [];

    public IReadOnlyCollection<AdminInventoryViewModel> LowStockBooks { get; set; } = [];
}
