using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ECommerceDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart?> GetCartWithItemsAsync(int userId)
    {
        return await _dbSet
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }
}
