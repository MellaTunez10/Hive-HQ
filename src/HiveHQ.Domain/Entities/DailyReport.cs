namespace HiveHQ.Domain.Entities;

public class DailyReport
{
    public Guid Id { get; set; }
    public DateTime ReportDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
