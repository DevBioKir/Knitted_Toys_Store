using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Http;

namespace Knitted_Toys_Store.App.Services
{
    public interface ICartService
    {
        Task<Guid> AddToCartAsync(Guid cartId, Guid toyId, int quantity);
        Task<Cart> CreateCartAsync();
        Task<Guid> DeleteCartAsync(Guid id);
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart?> GetCartByIdAsync(Guid id);
        Task<Cart> GetCurrentCartAsync(HttpContext httpContext, HttpResponse responce);
        Task<Guid> RemoveItemFromCartAsync(Guid cartId, Guid toyId);
        Task<Guid> UpdateAsync(Cart cart);
        Task<Guid> ReduceQuantityItemAsync(Guid cartId, Guid toyId);
        Task ClearCartAsync(Guid cartId);
        Task<Cart?> CloneCartAsync(Guid cartId);
    }
}