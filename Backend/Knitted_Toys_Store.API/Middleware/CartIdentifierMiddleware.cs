using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.API.Middleware
{
    public class CartIdentifierMiddleware
    {
        public const string CartCookieName = "cart_id"; //имя cookie, в которой будет храниться Guid корзины.
        private readonly RequestDelegate _next; //ссылк на следующий middleware в конвейере
        private readonly ILogger<CartIdentifierMiddleware> _logger;
        
        public CartIdentifierMiddleware(RequestDelegate next, ILogger<CartIdentifierMiddleware> logger)
        {
            _next = next; //сохраняем ссылку на следующий middleware
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ICartService cartService)
        {
            _logger.LogInformation($"CartIdentifierMiddleware: Обработка запроса {context.Request.Path}");

            //Проверка запроса
            bool isGetCartRequest = context.Request.Path.Value?.Equals("/Cart/Current", StringComparison.OrdinalIgnoreCase) == true &&
                                context.Request.Method == HttpMethods.Get;

            if (context.Items.ContainsKey(CartCookieName))
            {
                _logger.LogInformation("CartIdentifierMiddleware: Запрос уже обработан, пропускаем");
                await _next(context);
                return;
            }

            // Проверяем наличие cookie
            if (context.Request.Cookies.TryGetValue(CartCookieName, out var cookieValue) &&
                Guid.TryParse(cookieValue, out var cartId))
            {
                _logger.LogInformation($"CartIdentifierMiddleware: найден ID корзины {cartId}");

                var cart = await cartService.GetCartByIdAsync(cartId);
                if (cart != null)
                {
                    _logger.LogInformation($"CartIdentifierMiddleware: Найдена существующая корзина с ID {cartId}");
                    context.Items[CartCookieName] = cartId;
                }
                else
                {
                    _logger.LogWarning($"CartIdentifierMiddleware: Корзина с ID {cartId} не найдена в базе данных");

                    // ВАЖНОЕ ИЗМЕНЕНИЕ: Не создаем новую корзину здесь, если это запрос /Cart/Current
                    // Пусть контроллер создаст корзину
                    if (!isGetCartRequest)
                    {
                        _logger.LogInformation("CartIdentifierMiddleware: Это не запрос /Cart/Current, создаем новую корзину");
                        cartId = await CreateNewCartAsync(cartService, context);
                        context.Items[CartCookieName] = cartId;
                    }
                }
            }
            else if (!isGetCartRequest) // Создаем корзину только если это НЕ запрос /Cart/Current
            {
                _logger.LogInformation("CartIdentifierMiddleware: ID корзины не найден в cookie, создаем новую корзину");
                var newCartId = await CreateNewCartAsync(cartService, context);
                context.Items[CartCookieName] = newCartId;
            }

            await _next(context);
        }

        private async Task<Guid> CreateNewCartAsync(ICartService cartService, HttpContext httpContext)
        {
            _logger.LogInformation("CartIdentifierMiddleware: Создаем новую корзину");
            var newCart = await cartService.CreateCartAsync();

            var isDevelopment = httpContext.Request.Host.Host.Contains("localhost");

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
                Secure = !isDevelopment,
                //Secure = httpContext.Request.IsHttps
                Path = "/"
            };

            httpContext.Response.Cookies.Append(CartCookieName, newCart.Id.ToString(), options);
            _logger.LogInformation($"CartIdentifierMiddleware: Создана новая корзина с ID: {newCart.Id}");

            return newCart.Id;
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
