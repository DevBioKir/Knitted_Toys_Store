using Knitted_Toys_Store.App.Services;

namespace Knitted_Toys_Store.API.Middleware
{
    public class CartIdentifierMiddleware
    {
        public const string CartCookieName = "cart_id"; //имя cookie, в которой будет храниться Guid корзины.
        private readonly RequestDelegate _next; //ссылк на следующий middleware в конвейере     
        
        public CartIdentifierMiddleware(RequestDelegate next)
        {
            _next = next; //сохраняем ссылку на следующий middleware
        }

        public async Task InvokeAsync(HttpContext context, ICartService cartService)
        {
            //Проверка запроса
            bool isGetAllCartsRequest = context.Request.Path.Value.EndsWith("/Cart") &&
                context.Request.Method == "GET";

            Guid cartId;
            if (context.Request.Cookies.TryGetValue(CartCookieName, out var cookieValue) &&
                Guid.TryParse(cookieValue, out cartId))
            {
                var cart = await cartService.GetCartByIdAsync(cartId);
                if (cart == null)
                {
                    // Если корзина не найдена, но cookie существует, создаем новую корзину
                    // только если это не запрос на получение всех корзин
                    if (!isGetAllCartsRequest)
                    {
                        cartId = await CreateNewCartAsync(cartService, context);
                        context.Items[CartCookieName] = cartId;
                    }
                }
                else
                {
                    // Корзина найдена, добавляем её ID в контекст
                    context.Items[CartCookieName] = cartId;
                }
            }
            else if (!isGetAllCartsRequest)
            {
                // Если cookie нет и это не запрос на получение всех корзин,
                // создаем новую корзину
                cartId = await CreateNewCartAsync(cartService, context);
                context.Items[CartCookieName] = cartId;
            }

            await _next(context);
        }

        private async Task<Guid> CreateNewCartAsync(ICartService cartService, HttpContext httpContext)
        {
            var newCart = await cartService.CreateCartAsync();

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = httpContext.Request.IsHttps
            };

            httpContext.Response.Cookies.Append(CartCookieName, newCart.Id.ToString(), options);
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
