using AutoMapper;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.DataAccess.Repositories;
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
            if (cart.CartItems.Count == 0) throw new InvalidOperationException("Cart is empty");

            var orderItems = cart.CartItems.Select(ci =>
                OrderItems.Create(cart.Id, ci.ToyId, ci.Quantity, ci.Toy.Price)).ToList();

            var order = Order.Create(surname, name, phone, email, deliveryAddress, deliveryNotes, orderItems);

            var entityOrder = _mapper.Map<OrderEntity>(order);
            _context.Orders.Add(entityOrder);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var entityOrder = await _context.Orders.FindAsync(orderId);

            if (entityOrder == null) throw new InvalidOperationException("Order not found");

            entityOrder.Status = newStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<Guid> RemoveOrderAsync(Guid orderId)
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
    }
}
