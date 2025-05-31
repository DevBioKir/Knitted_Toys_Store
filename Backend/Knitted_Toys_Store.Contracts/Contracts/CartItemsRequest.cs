namespace Knitted_Toys_Store.Contracts
{
    public record CartItemsRequest(
        Guid Id,
        Guid CartId,
        Guid ToyId,
        int Quantity,
        DateTime AddedAt);
}
