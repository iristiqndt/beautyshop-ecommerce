using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<Product?> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> SearchAsync(string keyword);
}
