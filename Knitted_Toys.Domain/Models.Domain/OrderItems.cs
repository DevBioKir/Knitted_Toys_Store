using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knitted_Toys.Domain.Models.Domain
{
    public class OrderItems
    {
        public Guid Id { get; set; } = Guid.NewGuid();//уникальный идентификатор позиции
        public Guid OrderId { get; set; } //внешний ключ на Orders
        public Guid ToyId { get; set; } //внешний ключ на Toy
        public int Quantity { get; set; } //количество товара
        public decimal PriceAtTime { get; set; } //цена на момент заказа

        public required Order Order { get; set; } //ссылка на Orders
        public required Toy Toy { get; set; } //ссылка на Toy
    }
}
