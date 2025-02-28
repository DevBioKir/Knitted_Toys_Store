using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knitted_Toys.Domain.Models.Domain
{
    public class CartItems
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CartId { get; set; } //внешний ключ на Cart
        public Guid ToyId { get; set; } //внешний ключ на Toy
        public int Quantity { get; set; } //количество товара
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; //дата добавления в корзину

        public required Cart Cart { get; set; } //ссылка на Cart
        public required Toy Toy { get; set; } //ссылка на Toy
    }
}
}
