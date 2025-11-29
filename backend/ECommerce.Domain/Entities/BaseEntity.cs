namespace ECommerce.Domain.Entities;

/// <summary>
/// Base entity class with common properties for all domain entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Creation timestamp in UTC
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last update timestamp in UTC (null if never updated)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
