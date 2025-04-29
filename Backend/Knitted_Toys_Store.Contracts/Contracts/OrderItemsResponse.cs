namespace Knitted_Toys_Store.Contracts
{
    public record OrderItemsResponse(
        Guid OrderId,
        Guid ToyId,
        int Quantity,
        decimal PriceAtTime);
}
