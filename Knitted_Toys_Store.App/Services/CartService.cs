using Knitted_Toys_Store.DataAccess.Repositories;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepositories _cartRepositories;
        public CartService(ICartRepositories cartRepositories)
        {
            _cartRepositories = cartRepositories;
        }
        public async Task<List<Cart>> GetAllCarts()
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

        public async Task<Guid> UpdateItemQuantityAsync(Guid cartId, Guid toyId, int newQuantity)
        {
            await _cartRepositories.UpdateItemQuantityAsync(cartId, toyId, newQuantity);
            return toyId;
        }

        public async Task<Guid> RemoveItemFromCartAsync(Guid cartId, Guid toyId)
        {
            await _cartRepositories.RemoveItemFromCartAsync(cartId, toyId);
            return toyId;
        }
    }
}
