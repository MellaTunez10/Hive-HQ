using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveHQ.Domain.Interfaces;
using HiveHQ.Domain.Entities;
using HiveHQ.Domain.DTOs;
namespace HiveHQ.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IOrderRepository _orderRepo;
    private readonly IGenericRepository<InventoryItem> _inventoryRepo;
    private readonly IGenericRepository<BusinessService> _serviceRepo;

    public StatisticsController(
        IOrderRepository orderRepo,
        IGenericRepository<InventoryItem> inventoryRepo,
        IGenericRepository<BusinessService> serviceRepo)
    {
        _orderRepo = orderRepo;
        _inventoryRepo = inventoryRepo;
        _serviceRepo = serviceRepo;
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
}
