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

            CreateMap<Order, OrderEntity>().ReverseMap();

            CreateMap<CartItems, CartItemsEntity>().ReverseMap();

            CreateMap<OrderItems, OrderItemsEntity>().ReverseMap();

            // Маппинг между запросами (DTO) и доменными моделями

            // Маппинг CartRequest -> Cart
            CreateMap<CartRequest, Cart>()
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItemsRequest));

            // Маппинг Cart -> CartResponce
            CreateMap<Cart, CartResponce>()
                .ForMember(dest => dest.CartItemsResponces, opt => opt.MapFrom(src => src.CartItems));

            // Маппинг CartItemsRequest -> CartItems
            CreateMap<CartItemsRequest, CartItems>()
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt));

            // Маппинг CartItems -> CartItemsResponce
            CreateMap<CartItems, CartItemsResponce>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt));

            // Маппинг OrderRequest -> Order
            CreateMap<OrderRequest, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItemsRequest));

            // Маппинг Order -> OrderResponce
            CreateMap<Order, OrderResponce>()
                .ForMember(dest => dest.OrderItemsResponces, opt => opt.MapFrom(src => src.OrderItems));

            // Маппинг OrderItemsRequest -> OrderItems
            CreateMap<OrderItemsRequest, OrderItems>()
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt));

            // Маппинг OrderItems -> OrderItemsResponce
            CreateMap<OrderItems, OrderItemsResponce>()
                .ForMember(dest => dest.ToyId, opt => opt.MapFrom(src => src.ToyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt));
        }
    }
    }
}
