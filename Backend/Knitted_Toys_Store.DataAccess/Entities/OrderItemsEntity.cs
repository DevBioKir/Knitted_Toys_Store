namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class OrderItemsEntity
    {
        public Guid Id { get; set; } 
        public Guid OrderId { get; set; }
        public Guid ToyId { get; set; } 
        public int Quantity { get; set; } 
        public decimal PriceAtTime { get; set; } 

        public OrderEntity? Order { get; set; } 
        public ToyEntity? Toy { get; set; } 
    }
}
