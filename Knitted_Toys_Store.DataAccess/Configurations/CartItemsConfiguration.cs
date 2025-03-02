using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    public class CartItemsConfiguration : IEntityTypeConfiguration<CartItemsEntity>
    {
        public void Configure(EntityTypeBuilder<CartItems> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Quantity).IsRequired();
        }
    }
}
