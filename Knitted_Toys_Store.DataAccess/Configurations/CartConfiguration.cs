using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<CartEntity>
    {
        public void Configure(EntityTypeBuilder<CartEntity> builder)
        {
            builder.HasKey(c => c.Id);


            builder.Property(c => c.CreateAt)
                .IsRequired();
            builder.Property(c => c.LastUpdate)
                .IsRequired();
            builder.Property(c => c.TotalAmount)
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            //builder.HasIndex(c => c.SessionId); //индекс на SessionId для быстрого поиска

            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade); //если удалена корзина, то удаляются CartItems
        }
    }
}
