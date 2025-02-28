
namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class CartEntity
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; } //идентификатор сессии пользователя, если не сработает то вернуть int
        public DateTime CreateAt { get; set; } //Дата создания корзины
        public DateTime LastUpdate { get; set; } //Дата последнего обновления корзины
        public List<CartItemsEntity> CartItems { get; set; } = []; //у корзины может быть много Toy
    }
}
