using System.Security.Cryptography.X509Certificates;

namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class OrderItems
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ToyId { get; private set; } 
        public int Quantity { get; private set; } 
        public decimal PriceAtTime { get; private set; } 

        public Order? Order { get; private set; } 
        public Toy? Toy { get; private set; } 

        public static OrderItems Create(Guid orderId, Guid toyId, int quantity, decimal priceAtTime)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (priceAtTime < 0)
                throw new ArgumentException("Price must be non-negative.");

            return new OrderItems
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ToyId = toyId,
                Quantity = quantity,
                PriceAtTime = priceAtTime
            };
        }
        public void UpdateQuantity(int newQuantity)
        {
            if(newQuantity <= 0) 
                throw new ArgumentException("The number must be greater than 0");

            Quantity = newQuantity;
        }
    }
}

