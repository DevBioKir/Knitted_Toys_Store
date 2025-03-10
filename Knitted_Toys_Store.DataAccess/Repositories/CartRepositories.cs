using AutoMapper;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.DataAccess.Mapping;
using Knitted_Toys_Store.Domain.Abstractions;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public class CartRepositories
    {
        private readonly Knitted_Toys_StoreDBContext _context;
        private readonly IMapper _mapper;

        public CartRepositories(Knitted_Toys_StoreDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task<Cart?> GetCartAsync(Guid cartId)//////////////////////////////////////
        {
            var entitiesCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Toy)
            .FirstOrDefaultAsync(c => c.Id == cartId);

            return entitiesCart == null ? null : _mapper.Map<Cart>(entitiesCart);
        }

        public async Task<Guid> CreateCartAsync(Cart cart)
        {
            var entitiesCart = new CartEntity();

            await _context.Carts.AddAsync(entitiesCart);
            await _context.SaveChangesAsync();

            return entitiesCart.Id;
        }
    }
}
