using System.ComponentModel.DataAnnotations.Schema;
using Knitted_Toys_Store.Domain.Models.Domain;


namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class OrderEntity
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; } //дата создания заказа
        public decimal TotalAmount { get; set; } //общая сумма заказа

        [Column(TypeName = "varchar(20)")]
        public OrderStatus Status { get; set; }//статус заказа

        public string? SurnameCustomer { get; set; } = string.Empty; //Фамилия заказчика
        public string? NameCustomer { get; set; } = string.Empty; //Имя заказчика
        public string? PhoneNumber { get; set; } = string.Empty; //Номер заказчика
        public string? Email { get; set; } = string.Empty; //email заказчика
        public string? DeliveryAddress { get; set; } = string.Empty; //адрес доставки
        public string? DeliveryNotes { get; set; } = string.Empty;

        public List<OrderItemsEntity> OrderItems { get; set; } = []; //у одного заказа может быть много товаров
    }
}
