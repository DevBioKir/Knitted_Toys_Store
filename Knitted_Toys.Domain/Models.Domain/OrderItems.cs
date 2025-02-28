
namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class OrderItems
    {
        public Guid Id { get; } = Guid.NewGuid();//уникальный идентификатор позиции
        public Guid OrderId { get; } //внешний ключ на Orders
        public Guid ToyId { get; } //внешний ключ на Toy
        public int Quantity { get; } //количество товара
        public decimal PriceAtTime { get; } //цена на момент заказа

        public required Order Order { get; set; } //ссылка на Orders
        public required Toy Toy { get; set; } //ссылка на Toy
    }
}
