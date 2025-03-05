using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.DataAccess
{
    public class Knitted_Toys_StoreDBContext : DbContext
    {
        public Knitted_Toys_StoreDBContext(DbContextOptions<Knitted_Toys_StoreDBContext> options)
            : base(options) //наследуем options от базового класса DbContext
        {
            
        }
        public DbSet<ToyEntity> Toys { get; set; } //DbSet для взаимодействия с коллекциями
        public DbSet<CartEntity> Carts { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<CartItemsEntity> CartItems { get; set; }
        public DbSet<OrderItemsEntity> OrderItems { get; set; }
    }
}
