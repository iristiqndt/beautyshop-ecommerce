namespace ECommerce.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRepository<ECommerce.Domain.Entities.Role> Roles { get; }
    IProductRepository Products { get; }
    IRepository<ECommerce.Domain.Entities.Category> Categories { get; }
    ICartRepository Carts { get; }
    IRepository<ECommerce.Domain.Entities.CartItem> CartItems { get; }
    IOrderRepository Orders { get; }
    IRepository<ECommerce.Domain.Entities.OrderItem> OrderItems { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
