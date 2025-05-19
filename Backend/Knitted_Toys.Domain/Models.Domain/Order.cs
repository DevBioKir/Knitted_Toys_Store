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

        public static Order Create(
            string surname, string name, string phone, string email, string deliveryAddress,
            string deliveryNotes, List<OrderItems> orderItems)
        {
            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name and surname cannot be empty");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("The mail and the delivery address cannot be empty");

            if (orderItems == null || orderItems.Count == 0)
                throw new ArgumentException("Order must contain at least one item");

            //вычисляем сумму
            decimal totalAmount = orderItems.Sum(item => item.Quantity * item.PriceAtTime);

            return new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount, //при создании передаем вычисленную сумму
                Status = OrderStatus.Pending,
                SurnameCustomer = surname,
                NameCustomer = name,
                PhoneNumber = phone,
                Email = email,
                DeliveryAddress = deliveryAddress,
                OrderItems = orderItems
            };
        }
        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

        public void TotalAmountUpdate()
        {
            TotalAmount = OrderItems
                .Where(item => item.Toy != null)
                .Sum(item => item.Quantity + item.Toy.Price);
        }

        public void IncreaseItemQuantity(Guid toyId)
        {
            var item = OrderItems.FirstOrDefault(oi => oi.ToyId == toyId);

            if (item == null)
                throw new InvalidOperationException
                        ($"When you increace the quantity of the item{toyId} you are looking for, the item itself was not found in the order");

            item.UpdateQuantity(item.Quantity + 1);

            TotalAmountUpdate();
        }

        public void ReduceItemQuantity(Guid toyId)
        {
            var item = OrderItems.FirstOrDefault(oi => oi.ToyId == toyId);

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
            var item = 
        }
    }
}
