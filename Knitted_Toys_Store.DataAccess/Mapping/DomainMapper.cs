using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Mapping
{
    public static class DomainMapper
    {
        // Cart mapping
        public static Cart ToDomain(this CartEntity entity)
        {
            var cart = Cart.Create();
            // Use reflection or other means to set private properties
            typeof(Cart).GetProperty(nameof(Cart.Id))!
                .SetValue(cart, entity.Id);
            typeof(Cart).GetProperty(nameof(Cart.CreateAt))!
                .SetValue(cart, entity.CreateAt);
            typeof(Cart).GetProperty(nameof(Cart.LastUpdate))!
                .SetValue(cart, entity.LastUpdate);
            typeof(Cart).GetProperty(nameof(Cart.CartItems))!
                .SetValue(cart, entity.CartItems.Select(ci => ci.ToDomain()).ToList());

            return cart;
        }

        // CartItems mapping
        public static CartItems ToDomain(this CartItemsEntity entity)
        {
            var cartItem = CartItems.Create(entity.CartId, entity.ToyId, entity.Quantity);
            typeof(CartItems).GetProperty(nameof(CartItems.Id))!
                .SetValue(cartItem, entity.Id);
            typeof(CartItems).GetProperty(nameof(CartItems.AddedAt))!
                .SetValue(cartItem, entity.AddedAt);
            typeof(CartItems).GetProperty(nameof(CartItems.Toy))!
                .SetValue(cartItem, entity.Toy?.ToDomain());

            return cartItem;
        }

        // Order mapping
        public static Order ToDomain(this OrderEntity entity)
        {
            var order = Order.Create(
                entity.SurnameCustomer ?? string.Empty,
                entity.NameCustomer ?? string.Empty,
                entity.PhoneNumber ?? string.Empty,
                entity.Email ?? string.Empty,
                entity.DeliveryAddress ?? string.Empty,
                entity.DeliveryNotes ?? string.Empty,
                entity.OrderItems.Select(oi => oi.ToDomain()).ToList()
            );

            typeof(Order).GetProperty(nameof(Order.Id))!
                .SetValue(order, entity.Id);
            typeof(Order).GetProperty(nameof(Order.Status))!
                .SetValue(order, entity.Status);

            return order;
        }

        // OrderItems mapping
        public static OrderItems ToDomain(this OrderItemsEntity entity)
        {
            return OrderItems.Create(
                entity.OrderId,
                entity.ToyId,
                entity.Quantity,
                entity.PriceAtTime
            );
        }

        // Toy mapping
        public static Toy ToDomain(this ToyEntity entity)
        {
            return Toy.Create(
                entity.Name,
                entity.Description,
                entity.Size,
                entity.Price,
                entity.ImageUrl
            );
        }

        // Entity mapping methods (Domain to Entity)
        public static CartEntity ToEntity(this Cart domain)
        {
            return new CartEntity
            {
                Id = domain.Id,
                CreateAt = domain.CreateAt,
                LastUpdate = domain.LastUpdate,
                CartItems = domain.CartItems.Select(ci => ci.ToEntity()).ToList()
            };
        }

        public static CartItemsEntity ToEntity(this CartItems domain)
        {
            return new CartItemsEntity
            {
                Id = domain.Id,
                CartId = domain.CartId,
                ToyId = domain.ToyId,
                Quantity = domain.Quantity,
                AddedAt = domain.AddedAt,
                Cart = domain.Cart?.ToEntity() ?? throw new InvalidOperationException("Cart is required"),
                Toy = domain.Toy?.ToEntity() ?? throw new InvalidOperationException("Toy is required")
            };
        }

        public static OrderEntity ToEntity(this Order domain)
        {
            return new OrderEntity
            {
                Id = domain.Id,
                OrderDate = domain.OrderDate,
                TotalAmount = domain.TotalAmount,
                Status = domain.Status,
                SurnameCustomer = domain.SurnameCustomer,
                NameCustomer = domain.NameCustomer,
                PhoneNumber = domain.PhoneNumber,
                Email = domain.Email,
                DeliveryAddress = domain.DeliveryAddress,
                DeliveryNotes = domain.DeliveryNotes,
                OrderItems = domain.OrderItems.Select(oi => oi.ToEntity()).ToList()
            };
        }

        public static OrderItemsEntity ToEntity(this OrderItems domain)
        {
            return new OrderItemsEntity
            {
                Id = domain.Id,
                OrderId = domain.OrderId,
                ToyId = domain.ToyId,
                Quantity = domain.Quantity,
                PriceAtTime = domain.PriceAtTime,
                Order = domain.Order.ToEntity(),
                Toy = domain.Toy.ToEntity()
            };
        }

        public static ToyEntity ToEntity(this Toy domain)
        {
            return new ToyEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                Size = domain.Size,
                Price = domain.Price,
                ImageUrl = domain.ImageUrl
            };
        }
    }
}
