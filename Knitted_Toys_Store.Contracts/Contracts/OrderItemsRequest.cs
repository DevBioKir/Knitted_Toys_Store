namespace Knitted_Toys_Store.Contracts.Contracts
{
    public record OrderItemsRequest(
        Guid Id,
        Guid OrderId,
        Guid ToyId,
        int Quantity,
        decimal PriceAtTime);
}
