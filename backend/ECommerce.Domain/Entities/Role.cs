namespace ECommerce.Domain.Entities;

/// <summary>
/// User role entity (Admin, User, etc.)
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Role name (e.g., Admin, User, Manager)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Role description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Users assigned to this role
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}
