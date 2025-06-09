using Knitted_Toys_Store.App.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Knitted_Toys_Store.Infrastructure.Middleware
{
    public class CartIdentifierMiddleware
    {
        public const string CartCookieName = "cart_id"; //имя cookie, в которой будет храниться Guid корзины.
        private readonly RequestDelegate _next; //ссылк на следующий middleware в конвейере
        private readonly ILogger<CartIdentifierMiddleware> _logger;
        
        public CartIdentifierMiddleware(
            RequestDelegate next, 
            ILogger<CartIdentifierMiddleware> logger)
        {
            _next = next; //сохраняем ссылку на следующий middleware
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ICartService cartService)
        {
            _logger.LogInformation($"CartIdentifierMiddleware: Обработка запроса {context.Request.Path}");

            if (context.Items.ContainsKey(CartCookieName))
            {
                _logger.LogInformation("CartIdentifierMiddleware: Запрос уже обработан");
                await _next(context);
                return;
            }

            // Проверяем cookie
            if (context.Request.Cookies.TryGetValue(CartCookieName, out var cookieValue) &&
                Guid.TryParse(cookieValue, out var cartId))
            {
                _logger.LogInformation($"CartIdentifierMiddleware: найден ID корзины {cartId}");

                var cart = await cartService.GetCartByIdAsync(cartId);
                if (cart != null)
                {
                    _logger.LogInformation($"CartIdentifierMiddleware: Корзина {cartId} существует");
                    context.Items[CartCookieName] = cartId;
                }
                else
                {
                    _logger.LogWarning($"CartIdentifierMiddleware: Корзина {cartId} не найдена в БД");
                }
            }

            await _next(context);
        }
    }
    public static class CartIdentifierMiddlewareExtensions
    {
        public static IApplicationBuilder UseCartIdentifier(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CartIdentifierMiddleware>();
        }
    }
}
