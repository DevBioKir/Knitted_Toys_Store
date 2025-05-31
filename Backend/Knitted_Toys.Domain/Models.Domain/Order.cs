using System.Collections.Generic;
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
        public DateTime OrderDate { get; private set; } 
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; private set; }

        public string? SurnameCustomer { get; private set; } = string.Empty; 
        public string? NameCustomer { get; private set; } = string.Empty; 
        public string? PhoneNumber { get; private set; } = string.Empty; 
        public string? Email { get; private set; } = string.Empty; 
        public string? DeliveryAddress { get; private set; } = string.Empty; 
        public string? DeliveryNotes { get; private set; } = string.Empty;

        public List<OrderItems> OrderItems { get; private set; } = [];

        private Order(
            decimal totalAmount, string surname, string name, string phone, string email, string deliveryAddress,
            string deliveryNotes, IEnumerable<OrderItems> orderItems)
        {
            Id = Guid.NewGuid();
            OrderDate = DateTime.UtcNow;
            TotalAmount = totalAmount;
            Status = OrderStatus.Pending;
            SurnameCustomer = surname.Trim();
            NameCustomer = name.Trim();
            PhoneNumber = phone.Trim();
            Email = email.Trim();
            DeliveryAddress = deliveryAddress.Trim();
            DeliveryNotes = deliveryNotes.Trim();
            OrderItems = orderItems.ToList();
        }

        public static Order Create(
            string surname, string name, string phone, string email, string deliveryAddress,
            string deliveryNotes, IEnumerable<OrderItems> orderItems)
        {
            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name and surname cannot be empty");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("The mail and the delivery address cannot be empty");

            var orderItemsList = orderItems.ToList();
            var totalPriceItems = orderItemsList.Sum(x => x.PriceAtTime * x.Quantity);

            var order = new Order(totalPriceItems, surname, name, phone, email, deliveryAddress, deliveryNotes, orderItemsList);

            return order;
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
                .Sum(item => item.Quantity * item.PriceAtTime);
        }

        public void AddOrderItems(IEnumerable<OrderItems> items)
        {
            OrderItems.AddRange(items);
            TotalAmountUpdate();
        }

        public void AddItem(Guid orderId, Guid toyId, int quantity, decimal priceAtTime)
        {
            var existingItem = FindOrderItemsByToyId(toyId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                OrderItems.Add(Domain.OrderItems.Create(orderId, toyId, quantity, priceAtTime));
            }
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
