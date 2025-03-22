namespace Knitted_Toys_Store.Contracts
{
    public record CartResponce(
        Guid Id,
        DateTime CreateAt,
        DateTime LastUpdate,
        decimal TotalAmount,
        List<CartItemsResponce> CartItemsResponces,
        byte[] RowVersion);
}
