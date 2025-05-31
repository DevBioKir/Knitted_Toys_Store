namespace Knitted_Toys_Store.Contracts
{
    public record CartRequest(
        DateTime CreateAt,
        DateTime LastUpdate,
        decimal TotalAmount,
        List<CartItemsRequest> CartItemsRequest,
        byte[] RowVersion);
}
