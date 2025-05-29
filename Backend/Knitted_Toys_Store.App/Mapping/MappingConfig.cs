using Mapster;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.Contracts;
using System.Runtime.Serialization;


namespace Knitted_Toys_Store.App.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Toy
            config.NewConfig<Toy, Toy>()
                    .ConstructUsing(src => Toy.Create(
                        src.Name,
                        src.Description,
                        src.Size,
                        src.Price,
                        src.ImageUrl
                    ))
                .Map(dest => dest.Id, src => src.Id);

            config.NewConfig<ToyEntity, Toy>()
                .ConstructUsing(src => Toy.Create(
                        src.Name,
                        src.Description,
                        src.Size,
                        src.Price,
                        src.ImageUrl
                    ));

            config.NewConfig<Toy, ToyEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Size, src => src.Size)
                .Map(dest => dest.Price, src => src.Price)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            config.NewConfig<Toy, ToysResponse>();
            config.NewConfig<ToysRequest, Toy>();

            // CartItem
            config.NewConfig<CartItems, CartItems>()
                  .ConstructUsing(src => CartItems.Create(src.CartId, src.ToyId, src.Quantity))
                  .Map(dest => dest.Id, src => src.Id)
                  .Map(dest => dest.AddedAt, src => src.AddedAt)
                  .Map(dest => dest.Quantity, src => src.Quantity)
                  .Map(dest => dest.CartId, src => src.CartId)
                  .Map(dest => dest.ToyId, src => src.ToyId);

            config.NewConfig<CartItemsEntity, CartItems>()
                  .ConstructUsing(src => CartItems.Create(
                        src.CartId,
                        src.ToyId,
                        src.Quantity
                    ))
                  .Ignore(dest => dest.Cart);

            config.NewConfig<CartItems, CartItemsEntity>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.CartId, src => src.CartId)
                .Map(dest => dest.ToyId, src => src.ToyId)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.AddedAt, src => src.AddedAt)
                .Ignore(dest => dest.Cart); // тоже игнорируем, чтобы не было рекурсии

            config.NewConfig<CartItems, CartItemsResponse>()
                  .Map(dest => dest.ToyId, src => src.ToyId);

            config.NewConfig<CartItemsRequest, CartItems>()
                  .Map(dest => dest.ToyId, src => src.ToyId)
                  .Ignore(dest => dest.Toy);

            // Cart
            config.NewConfig<Cart, Cart>()
                   .ConstructUsing(src => Cart.Create())
                   .Map(dest => dest.CartItems, src => src.CartItems.Adapt<List<CartItems>>())
                   .Map(dest => dest.RowVersion, src => src.RowVersion);

            config.NewConfig<CartEntity, Cart>()
                  .ConstructUsing(src => Cart.Create())
                  .Map(dest => dest.CartItems, src => src.CartItems.Adapt<List<CartItems>>())
                  .Map(dest => dest.RowVersion, src => src.RowVersion);

            config.NewConfig<Cart, CartEntity>()
                  .Map(dest => dest.CartItems, src => src.CartItems.Adapt<List<CartItemsEntity>>())
                  .Map(dest => dest.RowVersion, src => src.RowVersion);

            config.NewConfig<Cart, CartResponse>()
                  .Map(dest => dest.CartItemsResponses, src => src.CartItems)
                  .Map(dest => dest.TotalAmount, src => src.TotalAmount);

            config.NewConfig<CartRequest, Cart>()
                  .Map(dest => dest.CartItems, src => src.CartItemsRequest)
                  .Ignore(dest => dest.Id)
                  .Ignore(dest => dest.TotalAmount)
                  .Ignore(dest => dest.RowVersion);

            // OrderItem
            config.NewConfig<OrderItems, OrderItems>()
                  .ConstructUsing(src => OrderItems.Create(
                      src.OrderId, 
                      src.ToyId, 
                      src.Quantity,
                      src.PriceAtTime))
                  .Map(dest => dest.Id, src => src.Id)
                  .Map(dest => dest.OrderId, src => src.OrderId)
                  .Map(dest => dest.ToyId, src => src.ToyId)
                  .Map(dest => dest.Quantity, src => src.Quantity)
                  .Map(dest => dest.PriceAtTime, src => src.PriceAtTime);

            config.NewConfig<OrderItemsEntity, OrderItems>()
                  .ConstructUsing(src => OrderItems.Create(
                      src.OrderId,
                      src.ToyId,
                      src.Quantity,
                      src.PriceAtTime))
                  .Ignore(dest => dest.Order);

            config.NewConfig<OrderItems, OrderItemsEntity>()
                  .Map(dest => dest.Id, src => src.Id)
                  .Map(dest => dest.OrderId, src => src.OrderId)
                  .Map(dest => dest.ToyId, src => src.ToyId)
                  .Map(dest => dest.Quantity, src => src.Quantity)
                  .Map(dest => dest.PriceAtTime, src => src.PriceAtTime)
                  .Ignore(dest => dest.Order); // тоже игнорируем, чтобы не было рекурсии
                  //.Ignore(dest => dest.Toy);

            config.NewConfig<OrderItems, OrderItemsResponse>()
                  .Map(dest => dest.ToyId, src => src.ToyId);

            config.NewConfig<OrderItemsRequest, OrderItems>()
                  .Map(dest => dest.ToyId, src => src.ToyId)
                  .Ignore(dest => dest.Toy);

            // Order
            config.NewConfig<OrderEntity, Order>()
                   .ConstructUsing(src => Order.Create(
                       src.SurnameCustomer,
                       src.NameCustomer,
                       src.PhoneNumber,
                       src.Email,
                       src.DeliveryAddress,
                       src.DeliveryNotes,
                       src.OrderItems.Adapt<List<OrderItems>>()));

            config.NewConfig<Order, OrderEntity>()
                .Map(dest => dest.OrderItems, src => src.OrderItems.Adapt<List<OrderItemsEntity>>())
                .Map(dest => dest.TotalAmount, src => src.TotalAmount);

            config.NewConfig<Order, OrderResponse>()
                .Map(dest => dest.OrderItemsResponse, src => src.OrderItems)
                .Map(dest => dest.TotalAmount, src => src.TotalAmount);

            config.NewConfig<OrderRequest, Order>()
                  .Map(dest => dest.OrderItems, src => src.OrderItemsRequest)
                  .Map(dest => dest.OrderDate, src => src.OrderDate)
                  .Map(dest => dest.Status, src => src.Status)
                  .Map(dest => dest.SurnameCustomer, src => src.SurnameCustomer)
                  .Map(dest => dest.NameCustomer, src => src.NameCustomer)
                  .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                  .Map(dest => dest.Email, src => src.Email)
                  .Map(dest => dest.DeliveryAddress, src => src.DeliveryAddress)
                  .Map(dest => dest.DeliveryNotes, src => src.DeliveryNotes)
                  .Ignore(dest => dest.Id)
                  .Ignore(dest => dest.TotalAmount);
        }
    }
}
