using System.Text.Json.Serialization;

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
            if (CartItems.Any(item => item.Toy == null))
                throw new InvalidOperationException("The toys are not loaded!");

            TotalAmount = CartItems.Sum(item => item.Quantity * item.Toy.Price);
        }

        public void UpdateItemQuantity(Guid toyId, int newQuantity)
        {
            var item = CartItems.FirstOrDefault(ci => ci.Toy?.Id == toyId);

            if (item == null) throw new InvalidOperationException("The toy was not found in the cart");

            item.UpdateQuantity(newQuantity);
            CartLastUpdate();
            TotalAmountUpdate();
        }
        public void RemoveItem(Guid toyId)
        {
            var item = CartItems.FirstOrDefault(ci => ci.Toy?.Id == toyId);

            if(item == null) throw new InvalidOperationException("The toy was not found in the cart");

            CartItems.Remove(item);
            CartLastUpdate();
            TotalAmountUpdate();
        }
    }
}
