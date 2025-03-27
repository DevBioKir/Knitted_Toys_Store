
using System.Text.Json.Serialization;

namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class CartEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; } //Дата создания корзины
        public DateTime LastUpdate { get; set; } //Дата последнего обновления корзины
        public decimal TotalAmount { get; set; }

        [JsonIgnore]
        public List<CartItemsEntity> CartItems { get; set; } = []; //у корзины может быть много Toy
        
        // Для оптимистичной блокировки, используем byte[]
        public byte[] RowVersion { get; set; }
    }
}
