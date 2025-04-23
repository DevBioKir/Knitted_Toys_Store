
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class OrderItemsEntity
    {
        public Guid Id { get; set; } //уникальный идентификатор позиции
        public Guid OrderId { get; set; } //внешний ключ на Orders
        public Guid ToyId { get; set; } //внешний ключ на Toy
        public int Quantity { get; set; } //количество товара
        public decimal PriceAtTime { get; set; } //цена на момент заказа

        public OrderEntity? Order { get; set; } //ссылка на Orders
        public ToyEntity? Toy { get; set; } //ссылка на Toy
    }
}
