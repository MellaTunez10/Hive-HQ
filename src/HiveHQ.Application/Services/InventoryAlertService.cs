using HiveHQ.Domain.Entities;
using HiveHQ.Domain.Interfaces;
using Microsoft.Extensions.Logging;

public class InventoryAlertService(IGenericRepository<InventoryItem> inventoryRepo, ILogger<InventoryAlertService> logger)
{
    public async Task CheckStockLevels()
    {
        var lowStockItems = await inventoryRepo.GetListAsync(i => i.QuantityInStock <= i.ReorderLevel);

        foreach (var item in lowStockItems)
        {
            // In Week 4, we will replace this log with a real Email/SMS
            logger.LogWarning($"ALERT: {item.Name} is low on stock! Current: {item.QuantityInStock}");
        }
    }
}
