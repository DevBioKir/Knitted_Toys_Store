using System.Text.Json.Serialization;

namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class CartItemsEntity
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; } 
        public Guid ToyId { get; set; } 
        public int Quantity { get; set; } 
        public DateTime AddedAt { get; set; } 

        [JsonIgnore]
        public CartEntity? Cart { get; set; } 

        [JsonIgnore]
        public ToyEntity? Toy { get; set; } 
    }
}
