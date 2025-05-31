using System.ComponentModel.DataAnnotations.Schema;
using Knitted_Toys_Store.Domain.Models.Domain;


namespace Knitted_Toys_Store.DataAccess.Entities
{
    public class OrderEntity
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "varchar(20)")]
        public OrderStatus Status { get; set; }

        public string? SurnameCustomer { get; set; } = string.Empty;
        public string? NameCustomer { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty; 
        public string? DeliveryAddress { get; set; } = string.Empty; 
        public string? DeliveryNotes { get; set; } = string.Empty;

        public List<OrderItemsEntity> OrderItems { get; set; } = []; 
    }
}
