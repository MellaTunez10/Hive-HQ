using HiveHQ.Domain.Entities;
namespace HiveHQ.Application.Interfaces
{
    public interface IOrderService
    {
        // Define the contract for creating an order
        Task<Guid> CreateOrderAsync(Guid serviceId, int quantity, string customerName);

        // Define the contract for getting all orders for today
        Task<IEnumerable<Order>> GetDailyOrdersAsync();
    }
}
