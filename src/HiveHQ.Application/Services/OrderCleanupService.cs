using HiveHQ.Domain.Interfaces;
using Microsoft.Extensions.Logging;

public class OrderCleanupService(IOrderRepository orderRepo, ILogger<OrderCleanupService> logger)
{
    public async Task ClearStaleOrders()
    {
        // Find orders created more than 24 hours ago that are still 'Pending'
        var threshold = DateTime.UtcNow.AddDays(-1);
        var staleOrders = await orderRepo.GetListAsync(o => o.Status == "Pending" && o.CreatedAt < threshold);

        foreach (var order in staleOrders)
        {
            order.Status = "Cancelled_System";
            orderRepo.Update(order);
        }

        if (staleOrders.Any())
        {
            await orderRepo.SaveChangesAsync();
            logger.LogInformation($"[Hangfire] Cleaned up {staleOrders.Count} stale orders.");
        }
    }
}
