namespace Knitted_Toys_Store.Contracts
{
    public record OrderItemsRequest(
        Guid Id,
        Guid OrderId,
        Guid ToyId,
        int Quantity,
        decimal PriceAtTime);
}
