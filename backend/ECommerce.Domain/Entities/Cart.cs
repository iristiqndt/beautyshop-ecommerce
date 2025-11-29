namespace ECommerce.Domain.Entities;

public class Cart : BaseEntity
{
    // Foreign Keys
    public int UserId { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    
    // Computed Property
    public decimal TotalAmount => CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
}
