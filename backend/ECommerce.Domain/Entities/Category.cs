namespace ECommerce.Domain.Entities;

/// <summary>
/// Product category entity
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Category description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Category image URL
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// URL-friendly category name for routes
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Products in this category
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
