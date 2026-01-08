using HiveHQ.Domain.Entities;
using HiveHQ.Domain.DTOs;

namespace HiveHQ.Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<decimal> GetTotalRevenueAsync(
        DateTime? start,
        DateTime? end);

    Task<List<TopServiceDto>> GetTopServicesAsync(int count, DateTime? start, DateTime? end);
}
