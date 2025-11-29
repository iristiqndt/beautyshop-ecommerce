using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Description)
            .HasMaxLength(2000);
        
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(p => p.StockQuantity)
            .IsRequired();
        
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(250);
        
        builder.HasIndex(p => p.Slug)
            .IsUnique();
        
        builder.Property(p => p.Brand)
            .HasMaxLength(100);
        
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);
        
        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
