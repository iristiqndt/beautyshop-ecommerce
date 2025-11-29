using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ECommerceDbContext _context;
    private IUserRepository? _users;
    private IRepository<Role>? _roles;
    private IProductRepository? _products;
    private IRepository<Category>? _categories;
    private ICartRepository? _carts;
    private IRepository<CartItem>? _cartItems;
    private IOrderRepository? _orders;
    private IRepository<OrderItem>? _orderItems;

    public UnitOfWork(ECommerceDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRepository<Role> Roles => _roles ??= new Repository<Role>(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);
    public IRepository<CartItem> CartItems => _cartItems ??= new Repository<CartItem>(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IRepository<OrderItem> OrderItems => _orderItems ??= new Repository<OrderItem>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
