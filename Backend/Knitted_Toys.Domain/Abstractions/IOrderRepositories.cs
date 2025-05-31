using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public interface IOrderRepositories
    {
        Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes);
        Task ReduceQuantityItemAsync(Guid orderId, Guid toyId);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> DeleteOrderAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<IEnumerable<Order>> SearchOrderAsync(
            string? surnameCustomer = null,
            string? nameCustomer = null,
            string? phoneNumber = null,
            string? email = null,
            string? deliveryAddress = null,
            OrderStatus? status = null);
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetOrderCountAsync();
        Task<IEnumerable<Order>> GetOrderByStatusAsync(OrderStatus status);
        Task<Guid> CloneOrderToCartAsync(Guid orderId);
        Task AddToOrderAsync(Guid orderId, Guid toyId, int quantity);
        Task RemoveItemFromOrderAsync(Guid orderId, Guid toyId);
    }
}