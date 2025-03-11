using AutoMapper;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Abstractions;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public class CartRepositories : ICartRepositories
    {
        private readonly Knitted_Toys_StoreDBContext _context;
        private readonly IMapper _mapper;

        public CartRepositories(Knitted_Toys_StoreDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Cart?> GetCarByIdtAsync(Guid cartId)
        {
            var entitiesCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Toy)
            .FirstOrDefaultAsync(c => c.Id == cartId);

            return entitiesCart == null ? null : _mapper.Map<Cart>(entitiesCart);
        }

        public async Task<Cart> CreateCartAsync()
        {
            var cart = Cart.Create();
            var entitiesCart = _mapper.Map<CartEntity>(cart);

            _context.Set<CartEntity>().Add(entitiesCart);
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task UpdateAsync(Cart cart)
        {
            cart.TotalAmountUpdate();
            var entityCart = _mapper.Map<CartEntity>(cart);
            _context.Set<CartEntity>().Update(entityCart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid cartId)
        {
            var entityCart = await _context.Set<CartEntity>().FindAsync(cartId);
            if (entityCart != null)
            {
                _context.Set<CartEntity>().Remove(entityCart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddToCartAsync(Guid cartId, Guid toyId, int quantity)
        {
            var cart = await GetCarByIdtAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");

            var entityToy = await _context.Set<ToyEntity>().FindAsync(toyId);
            if (entityToy == null) throw new InvalidOperationException("Toy not found");

            var toy = _mapper.Map<ToyEntity>(entityToy);
            var cartItem = CartItems.Create(cartId, toyId, quantity);
            cart.CartItems.Add(cartItem);

            await UpdateAsync(cart);
        }

        public async Task UpdateItemQuantityAsync(Guid cartId, Guid toyId, int newQuantity)
        {
            var cart = await GetCarByIdtAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");

            cart.UpdateItemQuantity(toyId, newQuantity);
            await UpdateAsync(cart);
        }
        public async Task RemoveItemFromCartAsync(Guid cartId, Guid toyId)
        {
            var cart = await GetCarByIdtAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");

            cart.RemoveItem(toyId);
            await UpdateAsync(cart);
        }
    }
}
