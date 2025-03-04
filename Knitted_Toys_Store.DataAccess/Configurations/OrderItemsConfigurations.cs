using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    class OrderItemsConfigurations : IEntityTypeConfiguration<OrderItemsEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemsEntity> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Quantity)
                .IsRequired();
            builder.Property(oi => oi.PriceAtTime)
                .HasPrecision(18, 2);

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Toy)
                .WithMany(t => t.OrderItems)
                .HasForeignKey(oi => oi.ToyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
