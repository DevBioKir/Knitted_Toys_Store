using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knitted_Toys.Domain.Models.Domain
{
    
    public enum OrderStatus
    {
        Pending,    // Ожидает оплаты
        Paid,       // Оплачен
        Shipped,    // Отправлен
        Delivered,  // Доставлен
        Cancelled   // Отменен
    }

    public class Order
    {
        public Order(decimal totalAmount, string surname, string name, string phone, string email, string deliveryAddress, string deliveryNotes)
        {
            TotalAmount = totalAmount;
            SurnameCustomer = surname;
            NameCustomer = name;
            PhoneNumber = phone;
            Email = email;
            DeliveryAddress = deliveryAddress;
            DeliveryNotes = deliveryNotes;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;//дата создания заказа
        public decimal TotalAmount { get; private set; } //общая сумма заказа
        public OrderStatus Status { get; private set; } = OrderStatus.Pending; //статус заказа

        public string? SurnameCustomer { get; private set; } = string.Empty; //Фамилия заказчика
        public string? NameCustomer { get; private set; } = string.Empty; //Имя заказчика
        public string? PhoneNumber { get; private set; } = string.Empty; //Номер заказчика
        public string? Email { get; private set; } = string.Empty; //email заказчика
        public string? DeliveryAddress { get; private set; } = string.Empty; //адрес доставки
        public string? DeliveryNotes { get; private set; } = string.Empty;

        public List<OrderItems> OrderItems { get; set; } = []; //у одного заказа может быть много товаров
    }
}
