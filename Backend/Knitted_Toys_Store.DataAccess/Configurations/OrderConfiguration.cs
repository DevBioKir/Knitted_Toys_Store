using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status) //конвертация статуса из enum в string для хранения в БД
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
            builder.Property(o => o.OrderDate).IsRequired();
            
            
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}
