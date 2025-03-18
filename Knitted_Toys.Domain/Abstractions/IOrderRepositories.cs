using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Repositories
//namespace Knitted_Toys_Store.Domain.Abstractions
{
    public interface IOrderRepositories
    {
        Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> RemoveOrderAsync(Guid orderId);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus); 
    }
}