using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    public class CartItemsConfiguration : IEntityTypeConfiguration<CartItemsEntity>
    {
        public void Configure(EntityTypeBuilder<CartItemsEntity> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Quantity).IsRequired();
        }
    }
}
