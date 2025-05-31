
using System.Text.Json.Serialization;

namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class CartEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdate { get; set; }
        public decimal TotalAmount { get; set; }

        [JsonIgnore]
        public List<CartItemsEntity> CartItems { get; set; } = [];
        
        public byte[] RowVersion { get; set; }
    }
}
