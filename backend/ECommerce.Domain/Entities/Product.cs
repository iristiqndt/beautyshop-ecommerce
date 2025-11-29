namespace ECommerce.Domain.Entities;

/// <summary>
/// Product entity for e-commerce catalog
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Product price in currency
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Available stock quantity
    /// </summary>
    public int StockQuantity { get; set; }
    
    /// <summary>
    /// Product image URL
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// URL-friendly product name for routes
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Product brand name
    /// </summary>
    public string? Brand { get; set; }
    
    /// <summary>
    /// Indicates if product is featured/promoted
    /// </summary>
    public bool IsFeatured { get; set; } = false;
    
    /// <summary>
    /// Foreign key to Category
    /// </summary>
    public int CategoryId { get; set; }
    
    /// <summary>
    /// Product category
    /// </summary>
    public Category Category { get; set; } = null!;
    
    /// <summary>
    /// Cart items containing this product
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    
    /// <summary>
    /// Order items containing this product
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
