using HiveHQ.Domain.Entities;
using HiveHQ.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HiveHQ.Application.Services;

public class ReportingService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IGenericRepository<DailyReport> _reportRepo; // Added this
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(
        IOrderRepository orderRepo,
        IGenericRepository<DailyReport> reportRepo,
        ILogger<ReportingService> logger)
    {
        _orderRepo = orderRepo;
        _reportRepo = reportRepo; // Assign the repository
        _logger = logger;
    }

    public async Task GenerateEndOfDayReport()
    {
        // We look at 'Yesterday' because we usually run this at 00:01 AM
        // to capture the full completed day.
        var yesterday = DateTime.UtcNow.AddDays(-1).Date;
        var start = yesterday;
        var end = yesterday.AddDays(1).AddTicks(-1);

        // 1. Calculate stats from the Orders table using your existing repo logic
        var revenue = await _orderRepo.GetTotalRevenueAsync(start, end);
        var orderCount = await _orderRepo.CountAsync(o => o.CreatedAt >= start && o.CreatedAt <= end);

        // 2. Create the Report record
        var report = new DailyReport
        {
            Id = Guid.NewGuid(),
            ReportDate = yesterday,
            TotalRevenue = revenue,
            TotalOrders = orderCount,
            GeneratedAt = DateTime.UtcNow
        };

        // 3. Save to the new DailyReports table
        await _reportRepo.AddAsync(report);
        await _reportRepo.SaveChangesAsync();

        _logger.LogInformation($"[Hangfire] Permanent EOD Report saved for {yesterday:yyyy-MM-dd}. Revenue: {revenue:C}");
    }
}
