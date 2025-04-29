using Knitted_Toys_Store.Contracts.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.Contracts
{
    public record OrderResponse(
        Guid Id,
        DateTime OrderDate,
        decimal TotalAmount,
        OrderStatus Status,
        string SurnameCustomer,
        string NameCustomer,
        string PhoneNumber,
        string Email,
        string DeliveryAddress,
        string DeliveryNotes,
        List<OrderItemsResponse> OrderItemsResponce);
}
