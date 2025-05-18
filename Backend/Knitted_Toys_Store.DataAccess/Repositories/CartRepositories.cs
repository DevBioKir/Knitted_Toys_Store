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
            // Очищаем ChangeTracker перед началом операции
            _context.ChangeTracker.Clear();

            // Загружаем корзину с включенными CartItems
            var entityCart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Toy)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (entityCart == null)
            {
                throw new InvalidOperationException($"Cart with ID {cart.Id} not found.");
            }

            // Маппинг существующей корзины в доменную модель
            var domainCart = _mapper.Map<Cart>(entityCart);

            // Обновляем каждый товар в корзине на основе нового списка
            foreach (var newItem in cart.CartItems)
            {
                var existingItem = domainCart.CartItems.FirstOrDefault(ci => ci.ToyId == newItem.ToyId);

                if (existingItem != null)
                {
                    domainCart.SetItemQuantity(newItem.ToyId, newItem.Quantity);
                }
                else
                {
                    // Добавляем новый товар в корзину
                    var toyEntity = await _context.Toys.FirstOrDefaultAsync(t => t.Id == newItem.ToyId);
                    if (toyEntity == null)
                        throw new InvalidOperationException("Toy not found!");

                    var toy = _mapper.Map<Toy>(toyEntity);
                    var newCartItem = CartItems.Create(domainCart.Id, newItem.ToyId, newItem.Quantity);
                    newCartItem.SetToy(_mapper.Map<Toy>(toy)); // Привязка игрушки
                    domainCart.CartItems.Add(newCartItem);
                }
            }

            // Удаляем товары, которых больше нет в обновленной корзине
            var itemsToRemove = domainCart.CartItems
                .Where(existingItem => !cart.CartItems.Any(newItem => newItem.ToyId == existingItem.ToyId))
                .ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                domainCart.RemoveItem(itemToRemove.ToyId);
            }

            // Пересчитываем общую сумму и обновляем время изменения
            domainCart.TotalAmountUpdate();
            domainCart.CartLastUpdate();

            // Применяем изменения к сущности entityCart
            entityCart.TotalAmount = domainCart.TotalAmount;
            entityCart.LastUpdate = domainCart.LastUpdate;

            entityCart.CartItems.Clear();
            foreach (var cartItem in domainCart.CartItems)
            {
                var cartItemEntity = _mapper.Map<CartItemsEntity>(cartItem);
                cartItemEntity.Toy = await _context.Toys.FindAsync(cartItem.ToyId); // Загружаем игрушку из базы данных
                entityCart.CartItems.Add(cartItemEntity);
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();
            return entityCart.Id;
        }

        //public async Task<Guid> UpdateAsync(Cart cart)
        //{
        //    // Очищаем ChangeTracker перед началом операции
        //    _context.ChangeTracker.Clear();

        //    // Загружаем корзину с включенными CartItems
        //    var entityCart = await _context.Carts
        //        .Include(c => c.CartItems)
        //        .ThenInclude(ci => ci.Toy)
        //        .FirstOrDefaultAsync(c => c.Id == cart.Id);

        //    if (entityCart == null)
        //    {
        //        throw new InvalidOperationException($"Cart with ID {cart.Id} not found.");
        //    }

        //    // Обновляем время и общую сумму корзины
        //    cart.CartLastUpdate();

        //    // Обрабатываем элементы корзины
        //    // 1. Удаляем элементы, которых нет в обновленной корзине
        //    var itemsToRemove = entityCart.CartItems
        //        .Where(existingItem => !cart.CartItems.Any(newItem => newItem.Id == existingItem.Id))
        //        .ToList();

        //    foreach (var itemToRemove in itemsToRemove)
        //    {
        //        _context.CartItems.Remove(itemToRemove);
        //    }

        //    // 2. Обновляем существующие элементы и добавляем новые
        //    foreach (var newItem in cart.CartItems)
        //    {
        //        var existingItem = entityCart.CartItems.FirstOrDefault(ci => ci.Id == newItem.Id);

        //        if (existingItem != null)
        //        {
        //            // Обновляем существующий элемент
        //            existingItem.Quantity = newItem.Quantity;
        //        }
        //        else
        //        {
        //            // Добавляем новый элемент
        //            var cartItems = CartItems.Create(cart.Id, newItem.ToyId, newItem.Quantity);
        //            var entityCartItemsEntity = _mapper.Map<CartItemsEntity>(cartItems);

        //            var toy = await _context.Toys.FindAsync(newItem.ToyId);
        //            if (toy == null)
        //                throw new InvalidOperationException("Toy not found!");

        //            entityCartItemsEntity.Toy = toy;
        //            entityCart.CartItems.Add(entityCartItemsEntity);
        //        }
        //    }

        //    // Пересчитываем общую сумму после обработки всех элементов
        //    entityCart.TotalAmount = entityCart.CartItems
        //       .Where(item => item.Toy != null)
        //       .Sum(item => item.Quantity * item.Toy.Price);

        //    // Обновляем основные свойства корзины
        //    entityCart.TotalAmount = cart.TotalAmount;
        //    entityCart.LastUpdate = cart.LastUpdate;

        //    // Сохраняем изменения
        //    await _context.SaveChangesAsync();

        //    return entityCart.Id;
        //}

        //public async Task<Guid> UpdateAsync(Cart cart)
        //{
        //    // Очищаем ChangeTracker перед началом операции
        //    _context.ChangeTracker.Clear();

        //    // Загружаем корзину с включенными CartItems
        //    var entityCart = await _context.Carts
        //        .Include(c => c.CartItems)
        //        .ThenInclude(ci => ci.Toy)
        //        .FirstOrDefaultAsync(c => c.Id == cart.Id);

        //    if (entityCart == null)
        //    {
        //        throw new InvalidOperationException($"Cart with ID {cart.Id} not found.");
        //    }

        //    // Обновляем время и общую сумму корзины
        //    cart.CartLastUpdate();
        //    cart.TotalAmountUpdate();

        //    // Обновляем основные свойства корзины
        //    entityCart.LastUpdate = cart.LastUpdate;
        //    entityCart.TotalAmount = cart.TotalAmount;

        //    // Обрабатываем элементы корзины
        //    // 1. Удаляем элементы, которых нет в обновленной корзине
        //    var itemsToRemove = entityCart.CartItems
        //        .Where(existingItem => !cart.CartItems.Any(newItem => newItem.Id == existingItem.Id))
        //        .ToList();

        //    foreach (var itemToRemove in itemsToRemove)
        //    {
        //        _context.CartItems.Remove(itemToRemove);
        //    }

        //    // 2. Обновляем существующие элементы и добавляем новые
        //    foreach (var newItem in cart.CartItems)
        //    {
        //        var existingItem = entityCart.CartItems.FirstOrDefault(ci => ci.Id == newItem.Id);

        //        if (existingItem != null)
        //        {
        //            // Обновляем существующий элемент
        //            existingItem.Quantity = newItem.Quantity;
        //        }
        //        else
        //        {
        //            // Добавляем новый элемент
        //            var cartItems = CartItems.Create(cart.Id, newItem.ToyId, newItem.Quantity);
        //            var entityCartItemsEntity = _mapper.Map<CartItemsEntity>(cartItems);
        //            entityCart.CartItems.Add(entityCartItemsEntity);
        //        }
        //    }

        //    // Сохраняем изменения
        //    await _context.SaveChangesAsync();

        //    return entityCart.Id;
        //}

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
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Toy)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            var existingItem = entityCart.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (existingItem != null)
            {
                await AddToyAsync(cartId, toyId, quantity);
            }
            else if (existingItem == null)
            {
                await CreateToysInCartItems(cartId, toyId, quantity);
            }
        }
        
        
        private async Task AddToyAsync(Guid cartId, Guid toyId, int quantity)
        {
            try
            {
                var entityCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

                var cart = _mapper.Map<Cart>(entityCart);
                cart.IncreaseItemQuantity(toyId);

                var updatedEntity = _mapper.Map<CartEntity>(cart);
                _context.Entry(entityCart).CurrentValues.SetValues(updatedEntity);

                foreach (var item in updatedEntity.CartItems)
                {
                    item.Toy = null;
                }
                entityCart.CartItems = updatedEntity.CartItems;

                await _context.SaveChangesAsync();
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка при добавлении игрушки в позицию в корзине: {err.Message}", err);
            }
        }

        private async Task CreateToysInCartItems(Guid cartId, Guid toyId, int quantity)
        {
            try
            {
                var entityCart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.Id == cartId);
                var cart = _mapper.Map<Cart>(entityCart);

                var newCartItem = CartItems.Create(cartId, toyId, quantity);
                var newCartItemEntity = _mapper.Map<CartItemsEntity>(newCartItem);
                _context.CartItems.Add(newCartItemEntity);

                cart.CartLastUpdate();
                cart.TotalAmountUpdate();

                await _context.SaveChangesAsync();
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка при создании позиции в корзине: {err.Message}", err);
            }
        }


        //public async Task AddToCartAsync(Guid cartId, Guid toyId, int quantity)
        //{
        //    var cartWithItems = await _context.Carts
        //        .Include(c => c.CartItems)
        //        .FirstOrDefaultAsync(c => c.Id == cartId);

        //    if (cartWithItems == null)
        //        throw new InvalidOperationException("Cart not found");

        //    var toy = await _context.Toys
        //        .FirstOrDefaultAsync(t => t.Id == toyId);

        //    if (toy == null)
        //        throw new InvalidOperationException("Toy not found");


        //    var existingItem = cartWithItems.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        var newCartItems = CartItems.Create(cartId, toyId, quantity);

        //        var newCartItemsEntity = _mapper.Map<CartItemsEntity>(newCartItems);
        //        _context.CartItems.Add(newCartItemsEntity);
        //    }

        //    // Обновляем общую сумму корзины
        //    cartWithItems.LastUpdate = DateTime.UtcNow;
        //    cartWithItems.TotalAmount = cartWithItems.CartItems.Sum(ci => ci.Quantity * (ci.ToyId == toyId ? toy.Price :
        //                (ci.Toy != null ? ci.Toy.Price : 0)));

        //    await _context.SaveChangesAsync();
        //}

        public async Task ReduceQuantityItemAsync(Guid cartId, Guid toyId) //уменьшение товара на единицу
        {
            var entityCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (entityCart == null)
                throw new InvalidOperationException("Cart not found");

            var cart = _mapper.Map<Cart>(entityCart);
            cart.ReduceItemQuantity(toyId);

            var updatedEntity = _mapper.Map<CartEntity>(cart);
            _context.Entry(entityCart).CurrentValues.SetValues(updatedEntity);

            foreach (var item in updatedEntity.CartItems)
            {
                item.Toy = null;
            }

            entityCart.CartItems = updatedEntity.CartItems;

            await _context.SaveChangesAsync();
        }

        //public async Task ReduceQuantityItemAsync(Guid cartId, Guid toyId) //удаление единицы товара в позиции
        //{
        //    var cartWithItems = await _context.Carts
        //        .Include(c => c.CartItems)
        //        .FirstOrDefaultAsync(c => c.Id == cartId);

        //    if (cartWithItems == null)
        //        throw new InvalidOperationException("Cart not found");

        //    var toy = await _context.Toys
        //        .FirstOrDefaultAsync(t => t.Id == toyId);

        //    if (toy == null)
        //        throw new InvalidOperationException("Toy not found");

        //    var existingItem = cartWithItems.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
        //    if (existingItem != null)
        //    {
        //        if (existingItem.Quantity > 1)
        //        {
        //            existingItem.Quantity -= 1;
        //        }
        //        else
        //        {
        //            //удаляем поцизию в корзине если осталась 1 шт.
        //            _context.CartItems.Remove(existingItem);
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("Товар не найден в корзине");
        //    }

        //    // Обновляем общую сумму корзины
        //    cartWithItems.LastUpdate = DateTime.UtcNow;
        //    cartWithItems.TotalAmount = cartWithItems.CartItems
        //        .Sum(ci => ci.Quantity * (ci.Toy != null ? ci.Toy.Price : 0));

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //        return; // Успех, выходим из метода
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        Console.WriteLine("Concurrency conflict, retrying...");
        //    }
        //}
        public async Task SetItemQuantityAsync(Guid cartId, Guid toyId, int quantity)//добавить точное количество
        {
            var entityCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (entityCart == null)
                throw new InvalidOperationException("Cart not found");

            var cart = _mapper.Map<Cart>(entityCart);

            cart.SetItemQuantity(toyId, quantity);

            var updatedEntity = _mapper.Map<CartEntity>(cart);
            _context.Entry(entityCart).CurrentValues.SetValues(updatedEntity);
            entityCart.CartItems = updatedEntity.CartItems;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemFromCartAsync(Guid cartId, Guid toyId)//удаление позиции товара из корзины полностью
        {
            // Очищаем ChangeTracker перед началом операции
            _context.ChangeTracker.Clear();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (cartItem == null)
                throw new InvalidOperationException("Item not found in cart");

            _context.CartItems.Remove(cartItem);
            cart.LastUpdate = DateTime.UtcNow;

            // Загружаем цены товаров для расчета общей суммы
            var remainingToyIds = cart.CartItems
                .Where(ci => ci.ToyId != toyId)
                .Select(ci => ci.ToyId)
                .ToList();

            var toys = await _context.Toys
                .Where(t => remainingToyIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.Price);

            // Рассчитываем общую сумму
            cart.TotalAmount = cart.CartItems
                .Where(ci => ci.ToyId != toyId)
                .Sum(ci => ci.Quantity * (toys.ContainsKey(ci.ToyId) ? toys[ci.ToyId] : 0));

            await _context.SaveChangesAsync();
        }

        private async Task<CartEntity?> LoadCartWithItems(Guid cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Toy)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }
    }
}
