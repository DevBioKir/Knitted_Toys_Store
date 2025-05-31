using System.Text.Json.Serialization;

namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class CartItems
    {
        public Guid Id { get; private set; }
        public Guid CartId { get; private set; } 
        public Guid ToyId { get; private set; } 
        public int Quantity { get; private set; } 
        public DateTime AddedAt { get; private set; }

        [JsonIgnore]
        public Cart? Cart { get; private set; } 

        [JsonIgnore]
        public Toy? Toy { get; private set; } 

        private CartItems() {}

        public static CartItems Create(Guid cartId, Guid toyId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");

            return new CartItems
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ToyId = toyId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };
        }
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0) throw new ArgumentException("The number must be greater than 0");

            Quantity = newQuantity;
        }

        public void SetToy(Toy toy)
        {
            Toy = toy ?? throw new ArgumentNullException(nameof(toy), "Toy cannot be null");
        }
    }
}
