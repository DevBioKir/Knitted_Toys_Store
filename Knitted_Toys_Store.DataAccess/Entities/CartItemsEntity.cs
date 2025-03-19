namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class CartItemsEntity
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; } //внешний ключ на Cart
        public Guid ToyId { get; set; } //внешний ключ на Toy
        public int Quantity { get; set; } //количество товара
        public DateTime AddedAt { get; set; } //дата добавления в корзину
        public CartEntity? Cart { get; set; } //ссылка на Cart
        public ToyEntity? Toy { get; set; } //ссылка на Toy
    }
}
