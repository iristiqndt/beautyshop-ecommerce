namespace ECommerce.Domain.Entities;

/// <summary>
/// User entity for authentication and customer information
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User email address (unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// User full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// User phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// User profile avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }
    
    /// <summary>
    /// User address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Password reset token
    /// </summary>
    public string? ResetPasswordToken { get; set; }
    
    /// <summary>
    /// Password reset token expiry time
    /// </summary>
    public DateTime? ResetPasswordExpiry { get; set; }
    
    /// <summary>
    /// Email confirmation status
    /// </summary>
    public bool EmailConfirmed { get; set; } = false;
    
    /// <summary>
    /// Foreign key to Role
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// User role
    /// </summary>
    public Role Role { get; set; } = null!;
    
    /// <summary>
    /// User orders
    /// </summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    
    /// <summary>
    /// User shopping cart
    /// </summary>
    public Cart? Cart { get; set; }
}
