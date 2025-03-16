using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Repositories
//namespace Knitted_Toys_Store.Domain.Abstractions
{
    public interface ICartRepositories
    {
        Task AddToCartAsync(Guid cartId, Guid toyId, int quantity);
        Task<Cart> CreateCartAsync();
        Task<Guid> DeleteAsync(Guid cartId);
        Task<Cart?> GetCarByIdAsync(Guid cartId);
        Task RemoveItemFromCartAsync(Guid cartId, Guid toyId);
        Task<Guid> UpdateAsync(Cart cart);
        Task UpdateItemQuantityAsync(Guid cartId, Guid toyId, int newQuantity);
    }
}