using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knitted_Toys.Domain.Models.Domain
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SessionId { get; set; } //идентификатор сессии пользователя, если не сработает то вернуть int
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;  //Дата создания корзины
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;//Дата последнего обновления корзины
        public List<CartItems> CartItems { get; set; } = []; //у корзины может быть много Toy
    }
}
