namespace ECommerce.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Snapshot of product price at order time
    public decimal TotalPrice { get; set; }
    
    // Foreign Keys
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    // Navigation
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
