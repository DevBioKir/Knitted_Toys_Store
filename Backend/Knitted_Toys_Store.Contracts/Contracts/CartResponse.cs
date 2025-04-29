namespace Knitted_Toys_Store.Contracts
{
    public record CartResponse(
        Guid Id,
        DateTime CreateAt,
        DateTime LastUpdate,
        decimal TotalAmount,
        List<CartItemsResponse> CartItemsResponses,
        byte[] RowVersion);
}
