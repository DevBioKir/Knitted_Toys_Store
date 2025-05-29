using Knitted_Toys_Store.DataAccess.Repositories;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Knitted_Toys_Store.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepositories _orderRepositories;
        private readonly ICartRepositories _cartRepositories;
        private readonly ILogger<OrderIdentifierMiddleware> _logger;
        public OrderService(
            IOrderRepositories orderRepositories, 
            ICartRepositories cartRepositories,
            ILogger<OrderIdentifierMiddleware> logger)
        {
            _orderRepositories = orderRepositories;
            _cartRepositories = cartRepositories;
            _logger = logger;
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

        public async Task<Guid> ReduceQuantityItemAsync(Guid orderId, Guid toyId) //удаление товара по единице в позиции
        {
            await _orderRepositories.ReduceQuantityItemAsync(orderId, toyId);
            return orderId;
        }

        public async Task<Order?> GetCurrentOrderAsync(HttpContext context, HttpResponse response)
        {
            _logger.LogInformation("Получен запрос на текущий заказ");

            if (context.Items.TryGetValue(OrderIdentifierMiddleware.OrderCookieName, out var orderIdObj) &&
                orderIdObj is Guid orderGuid)
            {
                _logger.LogInformation($"Найден ID заказа в HttpContext.Items: {orderGuid}");

                var order = await _orderRepositories.GetOrderByIdAsync(orderGuid);
                if (order != null)
                {
                    return order;
                }
            }

            if (context.Request.Cookies.TryGetValue(OrderIdentifierMiddleware.OrderCookieName, out string? cookieOrderIdStr) &&
                Guid.TryParse(cookieOrderIdStr, out Guid cookieOrderId))
            {
                _logger.LogInformation($"Найден ID заказа в cookie: {cookieOrderId}");

                var order = await _orderRepositories.GetOrderByIdAsync(cookieOrderId);
                if (order != null)
                {
                    return order;
                }
            }

            _logger.LogWarning("Не найден текущий заказ");

            return null;
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

        public async Task<Guid> AddToOrderAsync(Guid orderId, Guid toyId, int quantity)
        {
            await _orderRepositories.AddToOrderAsync(orderId, toyId, quantity);
            return toyId;
        }

        public async Task<Guid> RemoveItemFromOrderAsync(Guid orderId, Guid toyId)
        {
            await _orderRepositories.RemoveItemFromOrderAsync(orderId, toyId);
            return toyId;
        }
    }
}
