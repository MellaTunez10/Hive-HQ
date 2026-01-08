namespace HiveHQ.Domain.DTOs;

public class DashboardStatsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int LowStockItemsCount { get; set; }

    public List<TopServiceDto> TopServices { get; set; } = new();
}
