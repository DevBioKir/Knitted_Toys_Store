using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.API.Contracts
{
    public record OrderResponce(
        Guid Id,
        DateTime OrderDate,
        decimal TotalAmount,
        OrderStatus Status,
        string SurnameCustomer,
        string NameCustomer,
        string PhoneNumber,
        string Email,
        string DeliveryAddress,
        string DeliveryNotes);
}
