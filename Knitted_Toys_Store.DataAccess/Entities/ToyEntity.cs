
namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class ToyEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;//путь к изображению

        public List<OrderItemsEntity> OrderItems { get; set; } = []; //у одной игрушки может быть много позиций в заказе
        public List<CartItemsEntity> CartItems { get; set; } = []; //у одной игрушки может быть много позиций в корзине
    }
}
