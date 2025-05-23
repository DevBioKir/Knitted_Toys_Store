using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> DeleteOrderAsync(Guid id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<OrderStatus> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<int> GetOrderCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<IEnumerable<Order>> SearchOrderAsync(
            string? surnameCustomer = null,
            string? nameCustomer = null,
            string? phoneNumber = null,
            string? email = null,
            string? deliveryAddress = null,
            OrderStatus? status = null);
        Task<IEnumerable<Order>> GetOrderByStatusAsync(OrderStatus status);
        Task<Guid> CloneOrderToCartAsync(Guid orderId);
    }
}