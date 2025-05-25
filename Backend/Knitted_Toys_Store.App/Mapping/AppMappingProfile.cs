using AutoMapper;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.Contracts;


namespace Knitted_Toys_Store.App.Mapping
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Toy, ToyEntity>().ReverseMap(); //первым передаем тип-источник значений, вторым – тип-приемник. Чтобы маппинг работал в обоих направлениях, используем .ReverseMap();

            CreateMap<Cart, CartEntity>().ReverseMap();

            CreateMap<OrderItemsEntity, OrderItems>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PriceAtTime, opt => opt.MapFrom(src => src.PriceAtTime))
                .ForMember(dest => dest.Toy, opt =>
                {
                    opt.PreCondition(src => src.Toy != null);
                    opt.MapFrom(src => src.Toy);
                })
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            CreateMap<OrderEntity, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<Order, OrderEntity>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItems, OrderItemsEntity>()
                .ForMember(dest => dest.Toy, opt => opt.Ignore()) // если не хочешь сохранять Toy как навигационное свойство
                .ForMember(dest => dest.Order, opt => opt.Ignore()); // чтобы избежать циклической зависимости

            //CreateMap<CartItems, CartItemsEntity>().ReverseMap();
            CreateMap<CartItemsEntity, CartItems>()
                 .ForMember(dest => dest.Toy, opt => opt.MapFrom(src => src.Toy));

            CreateMap<CartItems, CartItemsEntity>()
                 .ForMember(dest => dest.Toy, opt => opt.Ignore());

            // Маппинг между запросами (DTO) и доменными моделями

            // Маппинг CartRequest -> Cart
            CreateMap<CartRequest, Cart>()
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItemsRequest));

            // Маппинг Cart -> CartResponse
            CreateMap<Cart, CartResponse>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("CreateAt", opt => opt.MapFrom(src => src.CreateAt))
                .ForCtorParam("LastUpdate", opt => opt.MapFrom(src => src.LastUpdate))
                .ForCtorParam("TotalAmount", opt => opt.MapFrom(src => src.TotalAmount))
                .ForCtorParam("CartItemsResponses", opt => opt.MapFrom(src => src.CartItems))
                .ForCtorParam("RowVersion", opt => opt.MapFrom(src => src.RowVersion));

            // Маппинг CartItemsRequest -> CartItems
            CreateMap<CartItemsRequest, CartItems>()
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt));

            // Маппинг CartItems -> CartItemsResponse
            CreateMap<CartItems, CartItemsResponse>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt))
                .ForMember(dect => dect.ToyName, opt => opt.MapFrom(src => src.Toy != null ? src.Toy.Name : string.Empty)) ///
                .ForMember(dect => dect.ToyImageUrl, opt => opt.MapFrom(src => src.Toy != null ? src.Toy.ImageUrl : string.Empty));///
            



            // Маппинг OrderRequest -> Order
            CreateMap<OrderRequest, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItemsRequest));

            // Маппинг Order -> OrderResponse
            CreateMap<Order, OrderResponse>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("OrderDate", opt => opt.MapFrom(src => src.OrderDate))
                .ForCtorParam("TotalAmount", opt => opt.MapFrom(src => src.TotalAmount))
                .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
                .ForCtorParam("SurnameCustomer", opt => opt.MapFrom(src => src.SurnameCustomer!))
                .ForCtorParam("NameCustomer", opt => opt.MapFrom(src => src.NameCustomer!))
                .ForCtorParam("PhoneNumber", opt => opt.MapFrom(src => src.PhoneNumber!))
                .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email!))
                .ForCtorParam("DeliveryAddress", opt => opt.MapFrom(src => src.DeliveryAddress!))
                .ForCtorParam("DeliveryNotes", opt => opt.MapFrom(src => src.DeliveryNotes!))
                .ForCtorParam("OrderItemsResponse", opt => opt.MapFrom(src => src.OrderItems));

            // Маппинг OrderItemsRequest -> OrderItems
            CreateMap<OrderItemsRequest, OrderItems>()
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PriceAtTime, opt => opt.MapFrom(src => src.PriceAtTime));///

            // Маппинг OrderItems -> OrderItemsResponse
            CreateMap<OrderItems, OrderItemsResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PriceAtTime, opt => opt.MapFrom(src => src.PriceAtTime))
                .ForMember(dect => dect.ToyName, opt => opt.MapFrom(src => src.Toy != null ? src.Toy.Name : string.Empty)) ///
                .ForMember(dect => dect.ToyImageUrl, opt => opt.MapFrom(src => src.Toy != null ? src.Toy.ImageUrl : string.Empty));///
        }
    }
}
