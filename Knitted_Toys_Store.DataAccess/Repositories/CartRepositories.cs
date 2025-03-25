using AutoMapper;
using Knitted_Toys_Store.DataAccess.Entities;
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

        public async Task<List<Cart>> GetAllCartsAsync()
        {
            var entitiesCart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Toy)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<Cart>>(entitiesCart);
        }

        public async Task<Cart?> GetCartByIdAsync(Guid cartId)
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
            Console.WriteLine($"TotalAmount: {entitiesCart.TotalAmount}"); // отладка
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<Guid> UpdateAsync(Cart cart)
        {
            var entityCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (entityCart == null)
            {
                throw new InvalidOperationException($"Cart with ID {cart.Id} not found.");
            }

            cart.CartLastUpdate();
            cart.TotalAmountUpdate();

            _mapper.Map(cart, entityCart);
            _context.Carts.Update(entityCart);
            await _context.SaveChangesAsync();

            return entityCart.Id;
        }

        public async Task<Guid> DeleteAsync(Guid cartId)
        {
            var entityCart = await _context.Set<CartEntity>().FindAsync(cartId);
            if (entityCart != null)
            {
                _context.Set<CartEntity>().Remove(entityCart);
                await _context.SaveChangesAsync();
            }
            return cartId;
        }

        public async Task AddToCartAsync(Guid cartId, Guid toyId, int quantity)
        {
            var cartWithItems = await _context.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cartWithItems == null)
                throw new InvalidOperationException("Cart not found");

            var toy = await _context.Toys
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == toyId);

            if (toy == null)
                throw new InvalidOperationException("Toy not found");

            // Очищаем ChangeTracker для избежания конфликтов отслеживания
            _context.ChangeTracker.Clear();

            // Загружаем корзину ещё раз, для редактирования
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found after clearing context");

            // Проверяем, есть ли уже эта игрушка в корзине
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItemsEntity
                {
                    Id = Guid.NewGuid(),
                    CartId = cartId,
                    ToyId = toyId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                });
            }

            // Обновляем общую сумму корзины
            cart.LastUpdate = DateTime.UtcNow;
            cart.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * (toy.Id == ci.ToyId ? toy.Price : 0));

            try
            {
                await _context.SaveChangesAsync();
                return; // Успех, выходим из метода
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Concurrency conflict, retrying...");
            }
        }

        public async Task UpdateItemQuantityAsync(Guid cartId, Guid toyId, int newQuantity)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");

            cart.UpdateItemQuantity(toyId, newQuantity);
            await UpdateAsync(cart);
        }
        public async Task RemoveItemFromCartAsync(Guid cartId, Guid toyId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");

            cart.RemoveItem(toyId);
            await UpdateAsync(cart);
        }
    }
}
