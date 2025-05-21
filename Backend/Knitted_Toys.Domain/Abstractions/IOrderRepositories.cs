using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public interface IOrderRepositories
    {
        Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> RemoveOrderAsync(Guid orderId);
        Task<List<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<IEnumerable<Order>> SearchOrderAsync(
            string? surnameCustomer = null,
            string? nameCustomer = null,
            string? phoneNumber = null,
            string? email = null,
            string? deliveryAddress = null,
            OrderStatus? status = null);
    }
}