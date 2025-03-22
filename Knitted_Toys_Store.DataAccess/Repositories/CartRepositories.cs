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
            var entityCart = await _context.Carts
                .Include(c => c.CartItems) // Загружаем товары в корзине
                .ThenInclude(ci => ci.Toy) // Загружаем игрушки
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (entityCart == null) throw new InvalidOperationException("Cart not found");

            var entityToy = await _context.Toys
                //.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == toyId);

            if (entityToy == null)
                throw new InvalidOperationException("Toy not found");

            var cart = _mapper.Map<Cart>(entityCart);

            // Проверяем, не был ли изменен RowVersion корзины
            var dbCart = await _context.Carts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cartId);
            if (dbCart != null && dbCart.RowVersion != entityCart.RowVersion)
            {
                throw new InvalidOperationException("Concurrency conflict: The cart data has been modified by another process.");
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = CartItems.Create(cartId, toyId, quantity);
                // Логируем, чтобы убедиться, что toy не null
                Console.WriteLine($"Toy found: {entityToy.Name}, Id: {entityToy.Id}");
                // Устанавливаем Toy через метод SetToy
                newItem.SetToy(_mapper.Map<Toy>(entityToy)); // Здесь устанавливаем Toy
                cart.CartItems.Add(newItem);
            }
            cart.TotalAmountUpdate();

            _mapper.Map(cart, entityCart);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var databaseValues = entry.GetDatabaseValues();

                    if (databaseValues == null)
                    {
                        // Возможно, баг с EF, попробуем снова загрузить объект напрямую
                        var exists = await _context.Carts.AnyAsync(c => c.Id == cartId);
                        if (!exists)
                            throw new InvalidOperationException("The cart has been deleted by another user.");

                        throw new InvalidOperationException("Concurrency conflict: data was modified by another process.");
                    }

                    Console.WriteLine("Original Values:");
                    foreach (var property in entry.OriginalValues.Properties)
                    {
                        Console.WriteLine($"{property.Name}: {entry.OriginalValues[property]}");
                    }

                    Console.WriteLine("Database Values:");
                    foreach (var property in databaseValues.Properties)
                    {
                        Console.WriteLine($"{property.Name}: {databaseValues[property]}");
                    }

                    // Если данные есть, обновляем оригинальные значения и пробуем снова
                    entry.OriginalValues.SetValues(databaseValues);
                }

                // Пробуем сохранить после разрешения конфликта
                await _context.SaveChangesAsync();
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
