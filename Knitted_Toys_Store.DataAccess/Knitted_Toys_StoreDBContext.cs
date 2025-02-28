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
        public DbSet<Toy> Toys { get; set; } //DbSet для взаимодействия с коллекциями
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItems> CartItems { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }

    }
}
