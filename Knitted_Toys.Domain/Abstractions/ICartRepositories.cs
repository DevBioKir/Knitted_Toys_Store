using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.Domain.Abstractions
{
    public interface ICartRepositories
    {
        Task AddToCartAsync(Guid cartId, Guid toyId, int quantity);
        Task<Cart> CreateCartAsync();
        Task DeleteAsync(Guid cartId);
        Task<Cart?> GetCarByIdtAsync(Guid cartId);
        Task RemoveItemFromCartAsync(Guid cartId, Guid toyId);
        Task UpdateAsync(Cart cart);
        Task UpdateItemQuantityAsync(Guid cartId, Guid toyId, int newQuantity);
    }
}