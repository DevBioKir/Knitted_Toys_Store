namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class Cart
    {
        public Guid Id { get; private set; } //сохранять его в cookies
        public DateTime CreateAt { get; private set; } //Дата создания корзины
        public DateTime LastUpdate { get; private set; } //Дата последнего обновления корзины
        public decimal TotalAmount { get; private set; } = 0;

        //[JsonIgnore]
        public List<CartItems> CartItems { get; private set; } = []; //у корзины может быть много Toy

        // Для оптимистичной блокировки, используем byte[]
        public byte[] RowVersion { get; set; }

        public static Cart Create() //фабричный метод
        {
            return new Cart()
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow
            };
        }
        public void CartLastUpdate() //обновление времени последнего изменения
        {
            LastUpdate = DateTime.UtcNow;
        }
        
        public void TotalAmountUpdate()
        {
            TotalAmount = CartItems
                .Where(item => item.Toy != null)
                .Sum(item => item.Quantity * item.Toy.Price);
        }
        
        //public void TotalAmountUpdate()
        //{
        //    if (CartItems.Any(item => item.Toy == null))
        //        throw new InvalidOperationException("The toys are not loaded!");

        //    TotalAmount = CartItems.Sum(item => item.Quantity * item.Toy.Price);
        //}

        public void SetItemQuantity(Guid toyId, int quantity) //если надо полностью обновить корзину точным значением
        {
            if (quantity < 0)
                throw new ArgumentException("The number must be greater than 0");

            var item = CartItems.FirstOrDefault(ci => ci.Toy?.Id == toyId || ci.ToyId == toyId);

            if (item == null)
                throw new InvalidOperationException("The toy was not found in the cart");

            item.UpdateQuantity(quantity);
            CartLastUpdate();
            TotalAmountUpdate();
        }
        public void IncreaseItemQuantity(Guid toyId) //увеличение количества товара в позиции на единицу
        {
            var item = CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
            if (item == null)
                throw new InvalidOperationException
                    ($"When you increace the quantity of the item{toyId} you are looking for, the item itself was not found in the cart");

            item.UpdateQuantity(item.Quantity + 1);
            
            CartLastUpdate();
            TotalAmountUpdate();
        }

        public void ReduceItemQuantity(Guid toyId) //уменьшение количество товара в позиции на единицу
        {
            var item = CartItems.FirstOrDefault(ci => ci.ToyId == toyId);
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

            var item = CartItems.FirstOrDefault(ci => ci.ToyId == toyId);

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
            var item = CartItems.FirstOrDefault(ci => ci.ToyId == toyId || ci.Toy?.Id == toyId);

            if(item == null) throw new InvalidOperationException("The toy was not found in the cart");

            CartItems.Remove(item);
            CartLastUpdate();
            TotalAmountUpdate();
        }
    }
}
