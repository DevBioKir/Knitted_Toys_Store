namespace Knitted_Toys_Store.API
{
    public class Test
    {
        public async Task AddToCartAsync(Guid cartId, Guid toyId, int quantity)
        {
            var cartWithItems = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cartWithItems == null)
                throw new InvalidOperationException("Cart not found");

            var toy = await _context.Toys
                .FirstOrDefaultAsync(t => t.Id == toyId);

            if (toy == null)
                throw new InvalidOperationException("Toy not found");


            var existingItem = cartWithItems.CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newCartItems = CartItems.Create(cartId, toyId, quantity);

                var newCartItemsEntity = _mapper.Map<CartItemsEntity>(newCartItems);
                _context.CartItems.Add(newCartItemsEntity);
            }

            // Обновляем общую сумму корзины
            cartWithItems.LastUpdate = DateTime.UtcNow;
            cartWithItems.TotalAmount = cartWithItems.CartItems.Sum(ci => ci.Quantity * (ci.ToyId == toyId ? toy.Price :
                        (ci.Toy != null ? ci.Toy.Price : 0)));

            await _context.SaveChangesAsync();
        }
    }
}
