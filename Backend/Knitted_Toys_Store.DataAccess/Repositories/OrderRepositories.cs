using AutoMapper;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly Knitted_Toys_StoreDBContext _context;
        private readonly IMapper _mapper;

        public OrderRepositories(Knitted_Toys_StoreDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var entitiesOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Toy)
                .ToListAsync();
            return _mapper.Map<List<Order>>(entitiesOrders);
        }
        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            var entityOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Toy)
                .FirstOrDefaultAsync(o => o.Id == orderId);

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

        //public async Task<Order> CreateOrderAsync(Cart cart, string surname, string name, string phone, string email, string deliveryAddress,
        //    string deliveryNotes)
        //{
        //    if (cart.CartItems.Count == 0) throw new InvalidOperationException("Cart is empty");

        //    var orderItems = cart.CartItems.Select(ci =>
        //        OrderItems.Create(cart.Id, ci.ToyId, ci.Quantity, ci.Toy.Price)).ToList();

        //    var order = Order.Create(surname, name, phone, email, deliveryAddress, deliveryNotes, orderItems);

        //    var entityOrder = _mapper.Map<OrderEntity>(order);
        //    _context.Orders.Add(entityOrder);
        //    await _context.SaveChangesAsync();

        //    return order;
        //}

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
            var order = await GetOrderByIdAsync(orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found");

            // Создаем новую корзину
            var newCart = Cart.Create();

            foreach (var item in order.OrderItems)
            {
                var newCartItem = CartItems.Create(newCart.Id, item.ToyId, item.Quantity);
                newCartItem.SetToy(item.Toy); // если доменная модель требует игрушку
                newCart.CartItems.Add(newCartItem);
            }

            newCart.TotalAmountUpdate();

            var cartEntity = _mapper.Map<CartEntity>(newCart);
            await _context.Carts.AddAsync(cartEntity);
            await _context.SaveChangesAsync();

            return newCart.Id;
        }
    }
}
