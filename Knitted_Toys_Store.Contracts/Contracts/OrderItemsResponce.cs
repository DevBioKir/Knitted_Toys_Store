namespace Knitted_Toys_Store.Contracts.Contracts
{
    public record OrderItemsResponce(
        Guid OrderId,
        Guid ToyId,
        int Quantity,
        decimal PriceAtTime);
}
