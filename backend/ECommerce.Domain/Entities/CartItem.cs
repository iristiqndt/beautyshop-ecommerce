namespace ECommerce.Domain.Entities;

public class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    
    // Foreign Keys
    public int CartId { get; set; }
    public int ProductId { get; set; }
    
    // Navigation
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
