using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();
        
        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(o => o.TaxAmount)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(o => o.ShippingCity)
            .HasMaxLength(100);
        
        builder.Property(o => o.ShippingPhone)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(o => o.RecipientName)
            .IsRequired()
            .HasMaxLength(200);
        
        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
