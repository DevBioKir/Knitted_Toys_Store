using Knitted_Toys_Store.App.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Knitted_Toys_Store.Infrastructure.Middleware
{
    public class OrderIdentifierMiddleware
    {
        public const string OrderCookieName = "order_id";
        private readonly RequestDelegate _next;
        private readonly ILogger<OrderIdentifierMiddleware> _logger;

        public OrderIdentifierMiddleware(RequestDelegate next, ILogger<OrderIdentifierMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IOrderService orderService)
        {
            _logger.LogInformation($"OrderIdentifierMiddleware: Обработка запроса {context.Request.Path}");

            if (context.Items.ContainsKey(OrderCookieName))
            {
                _logger.LogInformation("OrderIdentifierMiddleware: Заказ уже обработан, пропускаем");
                await _next(context);
                return;
            }

            if (context.Request.Cookies.TryGetValue(OrderCookieName, out var cookieValue) &&
                Guid.TryParse(cookieValue, out var orderId))
            {
                _logger.LogInformation($"OrderIdentifierMiddleware: найден ID заказа {orderId}");

                var order = await orderService.GetOrderByIdAsync(orderId);
                if (order != null)
                {
                    _logger.LogInformation($"OrderIdentifierMiddleware: Найден существующий заказ с ID {orderId}");
                    context.Items[OrderCookieName] = orderId;
                }
                else
                {
                    _logger.LogWarning($"OrderIdentifierMiddleware: Заказ с ID {orderId} не найден");
                }
            }

            await _next(context);
        }
    }

    public static class OrderIdentifierMiddlewareExtensions
    {
        public static IApplicationBuilder UseOrderIdentifier(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OrderIdentifierMiddleware>();
        }
    }

}
