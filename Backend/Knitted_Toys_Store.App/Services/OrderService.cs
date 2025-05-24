using Knitted_Toys_Store.DataAccess.Repositories;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepositories _orderRepositories;
        private readonly ICartRepositories _cartRepositories;
        public OrderService(IOrderRepositories orderRepositories, ICartRepositories cartRepositories)
        {
            _orderRepositories = orderRepositories;
            _cartRepositories = cartRepositories;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepositories.GetAllOrdersAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepositories.GetOrderByIdAsync(orderId);
        }

        public async Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, 
            string email, string deliveryAddress, string deliveryNotes)
        {
            return await _orderRepositories.CreateOrderAsync(cart, surname, name, phone, email, deliveryAddress, deliveryNotes);
        }

        public async Task<OrderStatus> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            await _orderRepositories.UpdateOrderStatusAsync(orderId, newStatus);
            return newStatus;
        }

        public async Task<Guid> DeleteOrderAsync(Guid id)
        {
            await _orderRepositories.DeleteOrderAsync(id);
            return id;
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _orderRepositories.GetOrderCountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await GetTotalRevenueAsync();
        }

        public async Task<IEnumerable<Order>> SearchOrderAsync(
            string? surnameCustomer = null, 
            string? nameCustomer = null, 
            string? phoneNumber = null, 
            string? email = null, 
            string? deliveryAddress = null, 
            OrderStatus? status = null)
        {
            return await _orderRepositories.SearchOrderAsync(
                surnameCustomer,
                nameCustomer,
                phoneNumber,
                email,
                deliveryAddress,
                status);
        }

        public async Task<IEnumerable<Order>> GetOrderByStatusAsync(OrderStatus status)
        {
            return await _orderRepositories.GetOrderByStatusAsync(status);
        }

        public async Task<Guid> CloneOrderToCartAsync(Guid orderId)
        {
            return await _orderRepositories.CloneOrderToCartAsync(orderId);
        }
    }
}
