
namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class ToyEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public List<OrderItemsEntity> OrderItems { get; set; } = [];
        public List<CartItemsEntity> CartItems { get; set; } = []; 
    }
}
