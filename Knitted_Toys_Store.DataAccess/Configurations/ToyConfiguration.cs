using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    public class ToyConfiguration : IEntityTypeConfiguration<ToyEntity>
    {
        public void Configure(EntityTypeBuilder<ToyEntity> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired()
                .HasMaxLength(Toy.MAX_LENGTH_NAME);

            builder.Property(t => t.Description).IsRequired()
                .HasMaxLength(Toy.MAX_LENGTH_DESCRIPTION);

            builder.Property(t => t.Size).IsRequired()
                .HasMaxLength(Toy.MAX_LENGTH_SIZE);

            builder.Property(t => t.Price)
                .HasPrecision(18, 5);

            builder.Property(t => t.ImageUrl)
                .IsRequired();

            builder.HasMany(t => t.OrderItems)
                .WithOne(oi => oi.Toy)
                .HasForeignKey(oi => oi.ToyId)
                .OnDelete(DeleteBehavior.Cascade); //Удаляем все OrderItems с этой игрушкой
              //.OnDelete(DeleteBehavior.Restrict); //Если надо будет оставлять удаленную игрушку в заказе

            builder.HasMany(t => t.CartItems)
                .WithOne(ci => ci.Toy)
                .HasForeignKey(ci => ci.ToyId)
                .OnDelete(DeleteBehavior.Cascade); //Удаляем все CartItems с этой игрушкой
        }
    }
}
