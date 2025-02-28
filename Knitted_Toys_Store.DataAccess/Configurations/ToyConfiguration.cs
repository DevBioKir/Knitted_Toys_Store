using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Knitted_Toys_Store.DataAccess.Configurations
{
    public class ToyConfiguration : IEntityTypeConfiguration<ToyEntity>
    {
        public void Configure(EntityTypeBuilder<ToyEntity> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Description).IsRequired().HasMaxLength(3000);
            builder.Property(t => t.Size).IsRequired().HasMaxLength(20);
            builder.Property(t => t.Price).HasPrecision(18, 5);
            builder.Property(t => t.ImageUrl).IsRequired();

            builder.HasMany(t => t.OrderItems)
                .WithOne(oi => oi.Toy)
                .HasForeignKey(oi => oi.ToyId); //Добавить вид удаления каскадный например

            builder.HasMany(t => t.CartItems)
                .WithOne(ci => ci.Toy)
                .HasForeignKey(ci => ci.ToyId); //Добавить вид удаления каскадный например
        }
    }
}
