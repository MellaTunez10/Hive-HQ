using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveHQ.Domain.Interfaces;
using HiveHQ.Domain.Entities;
using HiveHQ.Domain.DTOs;


namespace HiveHQ.API.Controllers;

[Authorize(Roles = "Admin")] // Only Admins can access these stats
[ApiController]
[Route("api/[controller]")]
[Tags("Management & Analytics")] // This groups the endpoints in the UI
public class StatisticsController : ControllerBase
{
    private readonly IOrderRepository _orderRepo;
    private readonly IGenericRepository<InventoryItem> _inventoryRepo;
    private readonly IGenericRepository<BusinessService> _serviceRepo;
    private readonly IDistributedCache _cache;

    public StatisticsController(
        IOrderRepository orderRepo,
        IGenericRepository<InventoryItem> inventoryRepo,
        IGenericRepository<BusinessService> serviceRepo,
        IDistributedCache cache)
    {
        _orderRepo = orderRepo;
        _inventoryRepo = inventoryRepo;
        _serviceRepo = serviceRepo;
        _cache =  cache;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardStatsDto>> GetSummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var stats = new DashboardStatsDto
        {
            TotalRevenue = await _orderRepo.GetTotalRevenueAsync(startDate, endDate),
            TotalOrders = await _orderRepo.CountAsync(o =>
                (!startDate.HasValue || o.CreatedAt >= startDate) &&
                (!endDate.HasValue || o.CreatedAt <= endDate)),
            LowStockItemsCount = await _inventoryRepo.CountAsync(i => i.QuantityInStock <= i.ReorderLevel),
            // Now this is a direct, fast call!
            TopServices = await _orderRepo.GetTopServicesAsync(3, startDate, endDate)
        };

        return Ok(stats);
    }

    [HttpGet("daily-summary")]
    public async Task<ActionResult<DashboardStatsDto>> GetDailySummary()
    {
        const string cacheKey = "daily_summary_stats";

        // 1. Try to get the cached string from Redis
        var cachedJson = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedJson))
        {
            // Cache Hit: Return the data instantly
            var cachedStats = JsonSerializer.Deserialize<DashboardStatsDto>(cachedJson);
            return Ok(cachedStats);
        }

        // 2. Cache Miss: Do the heavy lifting from the DB
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1).AddTicks(-1);

        var stats = new DashboardStatsDto
        {
            TotalRevenue = await _orderRepo.GetTotalRevenueAsync(todayStart, todayEnd),
            TotalOrders = await _orderRepo.CountAsync(o => o.CreatedAt >= todayStart && o.CreatedAt <= todayEnd),
            LowStockItemsCount = await _inventoryRepo.CountAsync(i => i.QuantityInStock <= i.ReorderLevel),
            TopServices = await _orderRepo.GetTopServicesAsync(5, todayStart, todayEnd)
        };

        // 3. Store the result in Redis for 5 minutes
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        var jsonStats = JsonSerializer.Serialize(stats);
        await _cache.SetStringAsync(cacheKey, jsonStats, options);

        return Ok(stats);
    }
}
