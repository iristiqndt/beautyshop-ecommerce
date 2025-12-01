namespace ECommerce.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Paid,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty; // ORD-20231119-001
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    
    // Shipping Information
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    
    // Payment Information
    public string? PaymentMethod { get; set; } // PayPal, COD
    public DateTime? PaidAt { get; set; }
    
    // Foreign Keys
    public int UserId { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
