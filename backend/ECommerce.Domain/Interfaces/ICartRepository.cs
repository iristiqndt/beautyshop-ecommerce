using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> GetCartWithItemsAsync(int userId);
}
