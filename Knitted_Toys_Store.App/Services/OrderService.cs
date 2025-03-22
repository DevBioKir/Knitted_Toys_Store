using Knitted_Toys_Store.DataAccess.Repositories;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepositories _orderRepositories;
        public OrderService(IOrderRepositories orderRepositories)
        {
            _orderRepositories = orderRepositories;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
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

        public async Task<Guid> RemoveOrderAsync(Guid id)
        {
            await _orderRepositories.RemoveOrderAsync(id);
            return id;
        }
    }
}
