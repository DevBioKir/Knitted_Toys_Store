using AutoMapper;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.DataAccess.Mapping
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
        }
    }
}
