using MapsterMapper;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly Knitted_Toys_StoreDBContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderRepositories> _logger;

        public OrderRepositories(
            Knitted_Toys_StoreDBContext context, 
            IMapper mapper,
            ILogger<OrderRepositories> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var entitiesOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Toy)
                .ToListAsync();

            foreach (var order in entitiesOrders)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item == null)
                        Console.WriteLine($"Null OrderItem в заказе {order.Id}");
                    else if (item.Toy == null)
                        Console.WriteLine($"OrderItem без Toy: OrderId={order.Id}, ToyId={item.ToyId}");
                }
            }

            return _mapper.Map<List<Order>>(entitiesOrders);
        }
        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            var entityOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Toy)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (entityOrder != null)
            {
                entityOrder.OrderItems = entityOrder.OrderItems?
                    .Where(oi => oi != null && oi.Toy != null)
                    .ToList();
            }

            return entityOrder == null ? null : _mapper.Map<Order>(entityOrder);
        }

        public async Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress,
    string deliveryNotes)
        {
            if (cart.CartItems.Count == 0)
                throw new InvalidOperationException("Cart is empty");

            // Сначала создаём заказ с пустым списком товаров
            var order = Order.Create(surname, name, phone, email, deliveryAddress, deliveryNotes, new List<OrderItems>());

            // Теперь создаём OrderItems с правильным order.Id
            var orderItems = cart.CartItems.Select(ci =>
                OrderItems.Create(order.Id, ci.ToyId, ci.Quantity, ci.Toy.Price)).ToList();

            order.AddOrderItems(orderItems);
            //order.OrderItems.AddRange(orderItems); // добавляем товары в заказ

            // Пересчёт суммы заказа
            //order.TotalAmountUpdate();

            // Маппим и сохраняем
            var entityOrder = _mapper.Map<OrderEntity>(order);
            _context.Orders.Add(entityOrder);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> SearchOrderAsync(
            string? surnameCustomer = null,
            string? nameCustomer = null,
            string? phoneNumber = null,
            string? email = null,
            string? deliveryAddress = null,
            OrderStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Toy)
                .AsQueryable();

            query = query
                .Where(o => 
                (string.IsNullOrWhiteSpace(surnameCustomer) || o.SurnameCustomer.Contains(surnameCustomer)) &&
                (string.IsNullOrWhiteSpace(nameCustomer) || o.NameCustomer.Contains(nameCustomer)) &&
                (string.IsNullOrWhiteSpace(phoneNumber) || o.PhoneNumber.Contains(phoneNumber)) &&
                (string.IsNullOrWhiteSpace(email) || o.Email.Contains(email)) &&
                (string.IsNullOrWhiteSpace(deliveryAddress) || o.DeliveryAddress.Contains(deliveryAddress)) &&
                (!status.HasValue || o.Status == status));

            var resultQuery = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<List<Order>>(resultQuery);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Status == OrderStatus.Paid)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<IEnumerable<Order>> GetOrderByStatusAsync(OrderStatus status)
        {
            var ordersByStatus = await _context.Orders
                .Where(o => o.Status == status).ToListAsync();

            return _mapper.Map<List<Order>>(ordersByStatus);
        }

        //public async Task UpdateOrderFromCartAsync(Guid orderId, Guid cartId)
        //{
        //    var entityOrder = await _context.Orders
        //        .Include(o => o.OrderItems)
        //        .FirstOrDefaultAsync(o => o.Id == orderId);

        //    if (entityOrder == null) throw new InvalidOperationException("Order not found");
        //    var domainOrder = _mapper.Map<Order>(entityOrder);

        //    var cart = await _context.Carts
        //        .Include(c => c.CartItems)
        //        .ThenInclude(ci => ci.Toy)
        //        .FirstOrDefaultAsync(c => c.Id == cartId);

        //    if (cart == null) throw new InvalidOperationException("Cart not found");

        //    domainOrder.OrderItems.Clear();

        //    foreach (var ci in cart.CartItems)
        //    {
        //        domainOrder.OrderItems.Add(OrderItems.Create(order.Id, ci.ToyId, ci.Quantity, ci.Toy.Price));
        //    }

        //    order.UpdateTotalAmount();
        //    _context.Orders.Update(order);
        //    await _context.SaveChangesAsync();
        //}

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var entityOrder = await _context.Orders.FindAsync(orderId);

            if (entityOrder == null) throw new InvalidOperationException("Order not found");

            entityOrder.Status = newStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<Guid> DeleteOrderAsync(Guid orderId)
        {
            var entityOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (entityOrder == null) throw new InvalidOperationException("Order not found");

            _context.OrderItems.RemoveRange(entityOrder.OrderItems);
            _context.Orders.Remove(entityOrder);

            await _context.SaveChangesAsync();
            return orderId;
        }

        public async Task<Guid> CloneOrderToCartAsync(Guid orderId)
        {
            _logger.LogInformation("Начало клонирования заказа в корзину. OrderId: {OrderId}", orderId);

            var order = await GetOrderByIdAsync(orderId);

            if (order == null)
            {
                _logger.LogWarning("Заказ с ID {OrderId} не найден", orderId);
                throw new InvalidOperationException("Order not found");
            }

            // Создаем новую корзину
            var newCart = Cart.Create();
            _logger.LogInformation("Создана новая корзина с ID {CartId}", newCart.Id);

            foreach (var item in order.OrderItems)
            {
                _logger.LogInformation(
                    "Обработка позиции заказа: ToyId={ToyId}, Quantity={Quantity}", item.ToyId, item.Quantity);

                var newCartItem = CartItems.Create(newCart.Id, item.ToyId, item.Quantity);
                if (item.Toy == null)
                {
                    _logger.LogError("Игрушка с ID {ToyId} не загружена в заказе", item.ToyId);
                    throw new InvalidOperationException($"Игрушка с ID {item.ToyId} не загружена в заказе");
                }
                // var toyDomain = _mapper.Map<Toy>(item.Toy); // ToyEntity уже загружен через Include
                //newCartItem.SetToy(toyDomain);
                newCartItem.SetToy(item.Toy);

                newCart.CartItems.Add(newCartItem);
            }

            newCart.TotalAmountUpdate();
            _logger.LogInformation("Итоговая сумма корзины: {Total}", newCart.TotalAmount);


            var cartEntity = _mapper.Map<CartEntity>(newCart);
            await _context.Carts.AddAsync(cartEntity);

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                Console.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
            }
            await _context.SaveChangesAsync();

            _logger.LogInformation("Корзина успешно сохранена в базу. CartId: {CartId}", newCart.Id);

            return newCart.Id;
        }

        public async Task AddToOrderAsync(Guid orderId, Guid toyId, int quantity)
        {
            var entityOrder = await _context.Orders
                .Include(c => c.OrderItems)
                //.ThenInclude(ci => ci.Toy)
                .FirstOrDefaultAsync(c => c.Id == orderId)
                ?? throw new Exception($"Order with ID {orderId} not found.");

            var entityToy = await _context.Toys
                //.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == toyId)
            ?? throw new Exception($"Toy with ID {toyId} not found.");

            var existingItem = entityOrder.OrderItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (existingItem != null)
            {
                await AddToyAsync(entityOrder, entityToy, quantity);
            }
            else if (existingItem == null)
            {
                await CreateToysInOrderItems(entityOrder, entityToy, quantity);
            }
        }

        private async Task AddToyAsync(OrderEntity entityOrder, ToyEntity entityToy, int quantity)
        {
            try
            {
                var order = _mapper.Map<Order>(entityOrder);
                order.IncreaseItemQuantity(entityToy.Id);

                var updatedEntityOrder = _mapper.Map<OrderEntity>(order);
                _context.Entry(entityOrder).CurrentValues.SetValues(updatedEntityOrder);

                foreach (var item in updatedEntityOrder.OrderItems)
                {
                    item.Toy = null;
                }
                entityOrder.OrderItems = updatedEntityOrder.OrderItems;

                await _context.SaveChangesAsync();
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка при добавлении игрушки в позицию в заказе: {err.Message}", err);
            }
        }

        private async Task CreateToysInOrderItems(OrderEntity entityOrder, ToyEntity entityToy, int quantity)
        {
            try
            {
                var order = _mapper.Map<Order>(entityOrder);

                var newOrderItems = OrderItems.Create(entityOrder.Id, entityToy.Id, quantity, entityToy.Price);
                var newOrderItemsEntity = _mapper.Map<OrderItemsEntity>(newOrderItems);
                _context.OrderItems.Add(newOrderItemsEntity);

                entityOrder.TotalAmount = entityOrder.OrderItems.Sum(ci => ci.Quantity * (ci.ToyId == entityToy.Id ? entityToy.Price :
                        (ci.Toy != null ? ci.Toy.Price : 0)));

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Ошибка при обновлении заказа: данные были изменены другим пользователем.");
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка при создании позиции в заказе: {err.Message}", err);
            }
        }
    }
}
