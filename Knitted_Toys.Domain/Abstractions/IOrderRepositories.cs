using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.Domain.Abstractions
{
    public interface IOrderRepositories
    {
        Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task RemoveOrder(Guid orderId);
        Task UpdateOrderStatus(Guid orderId, OrderStatus newStatus); 
    }
}