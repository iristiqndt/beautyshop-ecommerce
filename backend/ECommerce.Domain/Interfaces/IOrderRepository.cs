using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<Order?> GetByStripeSessionIdAsync(string sessionId);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<Order?> GetByIdWithItemsAsync(int id);
}
