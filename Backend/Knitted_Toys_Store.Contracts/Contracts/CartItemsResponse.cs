namespace Knitted_Toys_Store.Contracts
{
    public record CartItemsResponse(
        Guid CartId,
        Guid ToyId,
        int Quantity,
        DateTime AddedAt,
        string ToyName,
        string ToyImageUrl);
}
