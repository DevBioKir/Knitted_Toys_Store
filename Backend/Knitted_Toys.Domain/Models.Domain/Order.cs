using System.Runtime.CompilerServices;

namespace Knitted_Toys_Store.Domain.Models.Domain
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
        public Guid Id { get; private set; }
        public DateTime OrderDate { get; private set; } //дата создания заказа
        public decimal TotalAmount { get; private set; } //общая сумма заказа
        public OrderStatus Status { get; private set; } //статус заказа

        public string? SurnameCustomer { get; private set; } = string.Empty; //Фамилия заказчика
        public string? NameCustomer { get; private set; } = string.Empty; //Имя заказчика
        public string? PhoneNumber { get; private set; } = string.Empty; //Номер заказчика
        public string? Email { get; private set; } = string.Empty; //email заказчика
        public string? DeliveryAddress { get; private set; } = string.Empty; //адрес доставки
        public string? DeliveryNotes { get; private set; } = string.Empty;

        public List<OrderItems> OrderItems { get; private set; } = []; //у одного заказа может быть много товаров

        private Order() {}

        public static Order Create(
            string surname, string name, string phone, string email, string deliveryAddress,
            string deliveryNotes, IEnumerable<OrderItems> orderItems)
        {
            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name and surname cannot be empty");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("The mail and the delivery address cannot be empty");

            var orderItemsList = orderItems.ToList();

            return new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                TotalAmount = orderItemsList.Sum(x => x.PriceAtTime * x.Quantity), //при создании передаем вычисленную сумму
                Status = OrderStatus.Pending,
                SurnameCustomer = surname,
                NameCustomer = name,
                PhoneNumber = phone,
                Email = email,
                DeliveryAddress = deliveryAddress,
                OrderItems = orderItemsList
            };
        }

        private OrderItems FindOrderItemsByToyId(Guid toyId)
        {
            return OrderItems.FirstOrDefault(oi => oi.ToyId == toyId);
        }
        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

        public void TotalAmountUpdate()
        {
            TotalAmount = OrderItems
                .Where(item => item.Toy != null)
                .Sum(item => item.Quantity * item.Toy.Price);
        }

        public void AddOrderItems(IEnumerable<OrderItems> items)
        {
            OrderItems.AddRange(items);
            TotalAmountUpdate();
        }

        public void IncreaseItemQuantity(Guid toyId)
        {
            var item = FindOrderItemsByToyId(toyId);

            if (item == null)
                throw new InvalidOperationException
                        ($"When you increace the quantity of the item{toyId} you are looking for, the item itself was not found in the order");

            item.UpdateQuantity(item.Quantity + 1);

            TotalAmountUpdate();
        }

        public void ReduceItemQuantity(Guid toyId)
        {
            var item = FindOrderItemsByToyId(toyId);

            if (item == null)
                throw new InvalidOperationException
                        ($"When you reduce the quantity of the item{toyId} you are looking for, the item itself was not found in the order");

            if(item.Quantity > 1)
                item.UpdateQuantity(item.Quantity - 1);
            else OrderItems.Remove(item);

            TotalAmountUpdate();
        }

        public void RemoveItem(Guid toyId)
        {
            var item = FindOrderItemsByToyId(toyId);
            if(item == null) throw new InvalidOperationException("The toy was not found in the order");

            OrderItems.Remove(item);
            TotalAmountUpdate();
        }
    }
}
