
namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class CartItems
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid CartId { get; private set; } //внешний ключ на Cart
        public Guid ToyId { get; private set; } //внешний ключ на Toy
        public int Quantity { get; private set; } //количество товара
        public DateTime AddedAt { get; private set; }//дата добавления в корзину
        public Cart? Cart { get; private set; } //ссылка на Cart
        public Toy? Toy { get; private set; } //ссылка на Toy

        public static CartItems Create(Guid cartId, Guid toyId, int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Quantity must be greater than zero");

            return new CartItems
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ToyId = toyId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };
        }
    }
}
