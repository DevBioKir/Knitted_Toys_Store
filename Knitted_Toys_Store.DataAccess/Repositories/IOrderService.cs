using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> RemoveOrderAsync(Guid id);
        Task<OrderStatus> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    }
}