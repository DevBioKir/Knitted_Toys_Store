using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public interface ICartRepositories
    {
        Task AddToCartAsync(Guid cartId, Guid toyId, int quantity);
        //Task AddToyAsync(Guid cartId, Guid toyId, int quantity);
        //Task CreateToysInCartItems(Guid cartId, Guid toyId, int quantity);
        Task<Cart> CreateCartAsync();
        Task<Guid> DeleteAsync(Guid cartId);
        Task<Cart?> GetCartByIdAsync(Guid cartId);
        Task<List<Cart>> GetAllCartsAsync();
        Task RemoveItemFromCartAsync(Guid cartId, Guid toyId);
        Task<Guid> UpdateAsync(Cart cart);
        Task ReduceQuantityItemAsync(Guid cartId, Guid toyId);
        Task ClearCartAsync(Guid cartId);
        Task<Cart?> CloneCartAsync(Guid cartId);
        //Task SetItemQuantityAsync(Guid cartId, Guid toyId, int quantity);
    }
}