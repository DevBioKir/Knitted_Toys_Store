using MapsterMapper;
using Knitted_Toys_Store.DataAccess.Repositories;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Knitted_Toys_Store.App.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepositories _cartRepositories;
        private readonly IMapper _mapper;
        private readonly ILogger<CartIdentifierMiddleware> _logger;
        public CartService(
            ICartRepositories cartRepositories, 
            IMapper mapper, 
            ILogger<CartIdentifierMiddleware> logger)
        {
            _cartRepositories = cartRepositories;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            return await _cartRepositories.GetAllCartsAsync();
        }

        public async Task<Cart?> GetCartByIdAsync(Guid id)
        {
            return await _cartRepositories.GetCartByIdAsync(id);
        }

        public async Task<Cart> CreateCartAsync()
        {
            return await _cartRepositories.CreateCartAsync();
        }

        public async Task<Cart> GetCurrentCartAsync(HttpContext httpContext, HttpResponse response)
        {
            _logger.LogInformation("Получен запрос на текущую корзину");

            // Проверяем HttpContext.Items (обработано middleware)
            if (httpContext.Items.TryGetValue(CartIdentifierMiddleware.CartCookieName, out var cartIdObj) &&
                cartIdObj is Guid cartGuid)
            {
                _logger.LogInformation($"Найден ID корзины в HttpContext.Items: {cartGuid}");
                var existingCart = await _cartRepositories.GetCartByIdAsync(cartGuid);
                if (existingCart != null)
                {
                    return existingCart;
                }
            }

            // Проверяем cookie напрямую
            if (httpContext.Request.Cookies.TryGetValue(CartIdentifierMiddleware.CartCookieName, out string? cookieCartIdStr) &&
                Guid.TryParse(cookieCartIdStr, out Guid cookieCartId))
            {
                _logger.LogInformation($"Найден ID корзины в cookie: {cookieCartId}");
                var existingCart = await _cartRepositories.GetCartByIdAsync(cookieCartId);
                if (existingCart != null)
                {
                    return existingCart;
                }
            }

            // Создаем новую корзину
            _logger.LogInformation("Создаем новую корзину");
            var newCart = await _cartRepositories.CreateCartAsync();

            // Настройки cookie для IP-адреса (HTTP)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false, 
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Path = "/",
                Domain = null, // Не указываем домен для IP
                SameSite = SameSiteMode.Lax, // Lax для HTTP
                Secure = false, // false для HTTP
                IsEssential = true
            };

            response.Cookies.Append(
                CartIdentifierMiddleware.CartCookieName, 
                newCart.Id.ToString(), 
                cookieOptions);

            httpContext.Items[CartIdentifierMiddleware.CartCookieName] = newCart.Id;

            _logger.LogInformation($"Создана новая корзина с ID: {newCart.Id}");
            return newCart;
        }

        public async Task<Guid> UpdateAsync(Cart cart)
        {
            return await _cartRepositories.UpdateAsync(cart);
        }

        public async Task<Guid> DeleteCartAsync(Guid id)
        {
            return await _cartRepositories.DeleteAsync(id);
        }

        public async Task<Guid> AddToCartAsync(Guid cartId, Guid toyId, int quantity)
        {
            await _cartRepositories.AddToCartAsync(cartId, toyId, quantity);
            return toyId;
        }

        public async Task<Guid> ReduceQuantityItemAsync(Guid cartId, Guid toyId) //удаление товара по единице в позиции
        {
            await _cartRepositories.ReduceQuantityItemAsync(cartId, toyId);
            return cartId;
        }

        public async Task<Guid> RemoveItemFromCartAsync(Guid cartId, Guid toyId) //удаление позиции полностью
        {
            await _cartRepositories.RemoveItemFromCartAsync(cartId, toyId);
            return toyId;
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            await _cartRepositories.ClearCartAsync(cartId);
        }

        public async Task<Cart?> CloneCartAsync(Guid cartId)
        {
            return await _cartRepositories.CloneCartAsync(cartId);  
        }
    }
}
