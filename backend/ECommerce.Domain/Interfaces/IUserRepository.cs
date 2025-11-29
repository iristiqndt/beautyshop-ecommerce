using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByResetTokenAsync(string token);
    Task<bool> EmailExistsAsync(string email);
}
