namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class Cart
    {
        public Guid Id { get; private set; } 
        public DateTime CreateAt { get; private set; } 
        public DateTime LastUpdate { get; private set; } 
        public decimal TotalAmount { get; private set; } = 0;

        public List<CartItems> CartItems { get; private set; } = []; 

        public byte[] RowVersion { get; set; }

        private Cart() {}

        public static Cart Create() 
        {
            return new Cart()
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow
            };
        }
        private CartItems FindCartItemsByToyId(Guid toyId)
        {
            return CartItems
                .Where(ci => ci.ToyId == toyId)
                .FirstOrDefault();
        }

        public void AddItem(Guid cartId, Guid toyId, int quantity)
        {
            var existingItem = FindCartItemsByToyId(toyId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                CartItems.Add(Domain.CartItems.Create(cartId, toyId, quantity));
            }
            TotalAmountUpdate();
            CartLastUpdate();
        }
        public void CreateItems(Guid cartId, Guid toyId, int quantity)
        {
            CartItems.Add(Domain.CartItems.Create(cartId, toyId, quantity));
            TotalAmountUpdate();
            CartLastUpdate();

        }
        public void CartLastUpdate()
        {
            LastUpdate = DateTime.UtcNow;
        }
        
        public void TotalAmountUpdate()
        {
            TotalAmount = CartItems
                .Where(item => item.Toy != null)
                .Sum(item => item.Quantity * item.Toy.Price);
        }

        public void SetItemQuantity(Guid toyId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("The number must be greater than 0");

            var item = FindCartItemsByToyId(toyId);

            if (item == null)
                throw new InvalidOperationException("The toy was not found in the cart");

            item.UpdateQuantity(quantity);
            CartLastUpdate();
            TotalAmountUpdate();
        }
        public void IncreaseItemQuantity(Guid toyId)
        {
            var item = FindCartItemsByToyId(toyId);

            if (item == null)
                throw new InvalidOperationException
                    ($"When you increace the quantity of the item{toyId} you are looking for, the item itself was not found in the cart");

            item.UpdateQuantity(item.Quantity + 1);
            
            CartLastUpdate();
            TotalAmountUpdate();
        }

        public void ReduceItemQuantity(Guid toyId)
        {
            var item = FindCartItemsByToyId(toyId);
            if (item == null)
                throw new InvalidOperationException
                    ($"When you reduce the quantity of the item {toyId} you are looking for, the item itself was not found in the cart");

            if (item.Quantity > 1)
                item.UpdateQuantity(item.Quantity - 1);
            else CartItems.Remove(item);

            CartLastUpdate();
            TotalAmountUpdate();
        }

        public void UpdateItemQuantity(Guid toyId, int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Quantity cannot be negative.");

            var item = FindCartItemsByToyId(toyId);

            if (item == null)
            {
                var newItem = Domain.CartItems.Create(Id, toyId, newQuantity);
                CartItems.Add(newItem);
            }
            else
            {
                item.UpdateQuantity(item.Quantity + newQuantity);
            }

            CartLastUpdate();
            TotalAmountUpdate();
        }

        public void RemoveItem(Guid toyId)
        {
            var item = FindCartItemsByToyId(toyId);

            if (item == null) throw new InvalidOperationException("The toy was not found in the cart");

            CartItems.Remove(item);
            CartLastUpdate();
            TotalAmountUpdate();
        }

        public Cart Clone()
        {
            var newCart = Create();

            foreach (var item in CartItems)
            {
                newCart.CartItems.Add(Domain.CartItems.Create(newCart.Id, item.ToyId, item.Quantity));
            }

            newCart.TotalAmountUpdate();

            return newCart;
        }

        public void Clear()
        {
            CartItems.Clear();
            CartLastUpdate();
            TotalAmountUpdate();
        }
    }
}
