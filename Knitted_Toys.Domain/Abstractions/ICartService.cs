using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public interface ICartService
    {
        Task<Guid> AddToCartAsync(Guid cartId, Guid toyId, int quantity);
        Task<Cart> CreateCartAsync();
        Task<Guid> DeleteCartAsync(Guid id);
        Task<Cart?> GetCartByIdAsync(Guid id);
        Task<Guid> RemoveItemFromCartAsync(Guid cartId, Guid toyId);
        Task<Guid> UpdateAsync(Cart cart);
        Task<Guid> UpdateItemQuantityAsync(Guid cartId, Guid toyId, int newQuantity);
    }
}