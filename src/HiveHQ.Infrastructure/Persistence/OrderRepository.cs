using HiveHQ.Domain.DTOs;
using HiveHQ.Domain.Entities;
using HiveHQ.Domain.Interfaces;
using HiveHQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HiveHQ.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<decimal> GetTotalRevenueAsync(
        DateTime? start,
        DateTime? end)
    {
        return await _context.Orders
            .Where(o =>
                (!start.HasValue || o.CreatedAt >= start) &&
                (!end.HasValue || o.CreatedAt <= end))
            .SumAsync(o => o.TotalPrice);
    }

    public async Task<List<TopServiceDto>> GetTopServicesAsync(
        int count,
        DateTime? start,
        DateTime? end)
    {
        return await _context.Orders
            .Where(o => (!start.HasValue || o.CreatedAt >= start) &&
                        (!end.HasValue || o.CreatedAt <= end))
            // Join with BusinessServices to get the Names
            .Join(_context.BusinessServices,
                order => order.BusinessServiceId,
                service => service.Id,
                (order, service) => new { order, service })
            .GroupBy(x => x.service.Name) // Group by the Name we just joined
            .Select(g => new TopServiceDto
            {
                ServiceName = g.Key,
                TimesOrdered = g.Count()
            })
            .OrderByDescending(x => x.TimesOrdered)
            .Take(count)
            .ToListAsync();
    }
}
